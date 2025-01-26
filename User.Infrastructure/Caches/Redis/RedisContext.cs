using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
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
		/// 事务集合
		/// </summary>
		private readonly Dictionary<int, ITransaction> transactions;

		/// <summary>
		/// redis上下文
		/// </summary>
		/// <param name="connectionPool">redis连接池</param>
		/// <param name="redisSettings">redis配置类</param>
		/// <param name="memoryCache">内存缓存</param>
		public RedisContext(RedisConnectionPool connectionPool, IOptionsSnapshot<RedisSettings> redisSettings, IMemoryCache memoryCache)
		{
			_connectionPool = connectionPool;
			_redisSettings = redisSettings.Value;
			_memoryCache = memoryCache;
			transactions = new Dictionary<int, ITransaction>();
		}

		#region redis相关的操作方法

		/// <summary>
		/// 获取redis中类型为string的数据
		/// </summary>
		/// <typeparam name="T">返回的类型</typeparam>
		/// <param name="key">缓存键</param>
		/// <param name="dbNum">数据库编号</param>
		/// <param name="preferLocal">优先查本地缓存</param>
		/// <returns></returns>
		public async Task<T> GetStringAsync<T>(string key, int dbNum = -1, bool preferLocal = false)
		{
			if(preferLocal && _memoryCache.TryGetValue(key, out T localCache))
			{
				return localCache;
			}

			var db = _connectionPool.GetDatabase(dbNum);
			var redisCache = await db.StringGetAsync(key, flags: CommandFlags.PreferReplica);
			return Deserialize<T>(redisCache);
		}

		/// <summary>
		/// 向redis插入string类型的数据
		/// </summary>
		/// <param name="key">缓存键</param>
		/// <param name="value">缓存值</param>
		/// <param name="expirationTime">过期时间</param>
		/// <param name="dbNum">数据库编号</param>
		/// <returns></returns>
		public async Task<bool> SetStringAsync(string key, object value, TimeSpan? expirationTime, int dbNum = -1)
		{
			if(expirationTime == null) expirationTime = TimeSpan.FromSeconds(_redisSettings.DefaultExpirationTime);
			var cache = Serialize(value);
			var transaction = GetTransaction(dbNum);

			bool result = false;
			if(transaction == null)
			{
				var db = _connectionPool.GetDatabase(dbNum);
				result = await db.StringSetAsync(key, cache, expirationTime);
			}
			else
			{
				transaction.StringSetAsync(key, cache, expirationTime);
			}

			return result;
		}

		/// <summary>
		/// 向redis管道发布消息
		/// </summary>
		/// <param name="channel">管道名称</param>
		///<param name="message">消息</param>
		/// <param name="dbNum">数据库</param>
		/// <returns></returns>
		public async Task PublishAsync(string channel, object message, int dbNum = -1)
		{
			var messageContent = Serialize(message);
			var transaction = GetTransaction(dbNum);

			if(transaction == null)
			{
				var db = _connectionPool.GetDatabase(dbNum);
				await db.PublishAsync(channel, messageContent);
			}
			else
			{
				transaction.PublishAsync(channel, messageContent);
			}
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

		/// <summary>
		/// 开启事务
		/// </summary>
		/// <param name="dbNum">数据库编号</param>
		/// <returns></returns>
		public ITransaction BeginTransaction(int dbNum = -1)
		{
			var transaction = GetTransaction(dbNum);

			if(transaction == null)
			{
				dbNum = dbNum == -1 ? _redisSettings.DefaultDbNumber : dbNum;
				var db = _connectionPool.GetDatabase(dbNum);
				transaction = db.CreateTransaction();
				transactions.Add(dbNum, transaction);
			}

			return transaction;
		}

		/// <summary>
		/// 提交执行事务
		/// </summary>
		/// <param name="transaction">事务对象</param>
		/// <returns></returns>
		public async Task<bool> CommitTransactionAsync(ITransaction transaction)
		{
			return await transaction.ExecuteAsync();
		}

		/// <summary>
		/// 获取事务
		/// </summary>
		/// <param name="dbNum">数据库编号</param>
		/// <returns></returns>
		public ITransaction? GetTransaction(int dbNum = -1)
		{
			var defaultDbNumber = dbNum == -1 ? _redisSettings.DefaultDbNumber : dbNum;

			var transaction = transactions
				.Where(p => p.Key == defaultDbNumber)
				.Select(p => p.Value)
				.FirstOrDefault();

			return transaction;
		}

		#endregion

		#region 私有方法

		/// <summary>
		/// 序列化
		/// </summary>
		/// <param name="value">待序列化的值</param>
		/// <returns></returns>
		private string Serialize(object value)
		{
			return System.Text.Json.JsonSerializer.Serialize(value); ;
		}

		/// <summary>
		/// 反序列化
		/// </summary>
		/// <typeparam name="T">返回的类型</typeparam>
		/// <param name="cacheData">从redis查询出来的数据</param>
		/// <returns></returns>
		private T Deserialize<T>(RedisValue cacheData)
		{
			if(cacheData.HasValue)
			{
				return JsonConvert.DeserializeObject<T>(cacheData);
			}
			else
			{
				return default(T);
			}
		}

		#endregion
	}
}
