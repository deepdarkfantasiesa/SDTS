using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using User.Infrastructure.Caches.Models.SyncMemoryCacheCommds;
using User.Infrastructure.Settings;

namespace User.Infrastructure.Caches.Redis
{
	/// <summary>
	/// redis上下文
	/// </summary>
	public class RedisContext: ICacheImpl
	{
		/// <summary>
		/// redis连接池
		/// </summary>
		private readonly RedisConnectionPool _connectionPool;

		/// <summary>
		/// redis配置类
		/// </summary>
		private RedisSettings _redisSettings;

		/// <summary>
		/// 内存缓存
		/// </summary>
		private readonly IMemoryCache _memoryCache;

		/// <summary>
		/// redis上下文
		/// </summary>
		/// <param name="connectionPool">redis连接池</param>
		/// <param name="redisSettings">redis配置类</param>
		/// <param name="memoryCache">内存缓存</param>
		public RedisContext(RedisConnectionPool connectionPool, IOptionsMonitor<RedisSettings> redisSettings, IMemoryCache memoryCache)
		{
			_connectionPool = connectionPool;
			redisSettings.OnChange(RedisSettingChange);
			_redisSettings = redisSettings.CurrentValue;
			_memoryCache = memoryCache;
		}

		/// <summary>
		/// RedisSettings配置内容变更触发函数
		/// </summary>
		/// <param name="redisSettings"></param>
		private async void RedisSettingChange(RedisSettings redisSettings)
		{
			_redisSettings = redisSettings;
		}

		#region redis相关的操作方法

		/// <summary>
		/// 获取redis中类型为string的数据
		/// </summary>
		/// <typeparam name="T">返回的类型</typeparam>
		/// <param name="cacheKey">缓存键</param>
		/// <param name="databaseNumber">数据库编号</param>
		/// <param name="preferLocal">优先查本地缓存</param>
		/// <returns></returns>
		public async Task<T> GetStringAsync<T>(string cacheKey, int? databaseNumber, bool preferLocal = false)
		{
			if(preferLocal && _memoryCache.TryGetValue(cacheKey, out T cache))
			{
				return cache;
			}

			var db = _connectionPool.GetDatabase(databaseNumber);
			var cacheData = await db.StringGetAsync(cacheKey,flags:CommandFlags.PreferReplica);
			return Deserialize<T>(cacheData);
		}

		/// <summary>
		/// 向redis插入string类型的数据
		/// </summary>
		/// <param name="cacheKey">缓存键</param>
		/// <param name="value">缓存值</param>
		/// <param name="expirationTime">过期时间</param>
		/// <param name="databaseNumber">数据库编号</param>
		/// <param name="publish">是否发布到通道</param>
		/// <returns></returns>
		public async Task<bool> SetStringAsync(string cacheKey, object value, TimeSpan? expirationTime, int? databaseNumber, bool publish = false)
		{
			if(expirationTime == null) expirationTime = TimeSpan.FromSeconds(_redisSettings.DefaultExpirationTime);

			//写入redis
			var db = _connectionPool.GetDatabase(databaseNumber);
			var redisValue = System.Text.Json.JsonSerializer.Serialize(value);
			var result = await db.StringSetAsync(cacheKey, redisValue, expirationTime);

			//判断是否发布缓存到channel
			if(publish == false || result == false || expirationTime < TimeSpan.FromSeconds(20)) return result;
			await SyncInMemoryCacheAsync(CacheKeyPrefix.SyncInMemoryCache, CommondType.Create, cacheKey, value, expirationTime / 2);
			return true;
		}

		/// <summary>
		/// 向redis管道发布消息
		/// </summary>
		/// <param name="channel">管道名称</param>
		/// <param name="databaseNumber">默认数据库</param>
		///<param name="value">值</param>
		/// <returns></returns>
		public async Task PublishAsync(string channel, object value, int? databaseNumber = null)
		{
			var db = _connectionPool.GetDatabase(databaseNumber);
			var redisValue = System.Text.Json.JsonSerializer.Serialize(value);
			
			await db.PublishAsync(channel, redisValue);
		}

		/// <summary>
		/// 订阅redis的管道
		/// </summary>
		/// <param name="channel">管道名称</param>
		/// <param name="handler">消费消息时的任务</param>
		/// <returns></returns>
		public async Task SubscribeAsync(string channel, Action<RedisChannel, RedisValue> handler)
		{
			var connection = _connectionPool.GetConnection();
			var subscriber = connection.GetSubscriber();
			await subscriber.SubscribeAsync(channel, handler);
		}

		#endregion

		#region 私有方法

		/// <summary>
		/// 反序列化
		/// </summary>
		/// <typeparam name="T">返回的类型</typeparam>
		/// <param name="cacheData">从redis查询出来的数据</param>
		/// <returns></returns>
		private T Deserialize<T>(RedisValue cacheData)
		{
			if (cacheData.HasValue)
			{
				return JsonConvert.DeserializeObject<T>(cacheData);
			}
			else
			{
				return default(T);
			}
		}

		/// <summary>
		/// 通过redis的pub/sub通知其他服务实例同步本地缓存
		/// </summary>
		/// <param name="type">消息类型</param>
		/// <param name="cacheKey">缓存键</param>
		/// <param name="data">缓存数据</param>
		/// <param name="channel">管道名称</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		private async Task SyncInMemoryCacheAsync(string channel, CommondType type, string cacheKey, object data, TimeSpan? expirationTime = null)
		{
			BaseCommand<object> command = null;
			switch (type)
			{
				case CommondType.Create:
					if (data == null || expirationTime == null) throw new ArgumentNullException("数据和过期时间不能为空");
					command = new CreateCommand<object>() { CacheKey = cacheKey, Data = data, ExpirationTime = expirationTime.Value, DataType = data.GetType().FullName };
					await PublishAsync(channel, command);
					break;
				case CommondType.Delete:
					command = new DeleteCommand<object>() { CacheKey = cacheKey };
					await PublishAsync(channel, command);
					break;
			}
		}

		#endregion
	}
}
