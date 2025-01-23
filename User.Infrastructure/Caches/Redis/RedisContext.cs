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
		/// <param name="cacheKey">缓存键</param>
		/// <param name="dbNum">数据库编号</param>
		/// <param name="preferLocal">优先查本地缓存</param>
		/// <returns></returns>
		public async Task<T> GetStringAsync<T>(string cacheKey, int dbNum = -1, bool preferLocal = false)
		{
			if(preferLocal && _memoryCache.TryGetValue(cacheKey, out T cache))
			{
				return cache;
			}

			var db = _connectionPool.GetDatabase(dbNum);
			var cacheData = await db.StringGetAsync(cacheKey, flags: CommandFlags.PreferReplica);
			return Deserialize<T>(cacheData);
		}

		/// <summary>
		/// 向redis插入string类型的数据
		/// </summary>
		/// <param name="cacheKey">缓存键</param>
		/// <param name="value">缓存值</param>
		/// <param name="expirationTime">过期时间</param>
		/// <param name="dbNum">数据库编号</param>
		/// <returns></returns>
		public async Task<bool> SetStringAsync(string cacheKey, object value, TimeSpan? expirationTime, int dbNum = -1)
		{
			if(expirationTime == null) expirationTime = TimeSpan.FromSeconds(_redisSettings.DefaultExpirationTime);
			var redisValue = Serialize(value);
			var transaction = GetTransaction(dbNum);

			bool result = false;
			if(transaction == null)
			{
				var db = _connectionPool.GetDatabase(dbNum);
				result = await db.StringSetAsync(cacheKey, redisValue, expirationTime);
			}
			else
			{
				transaction.StringSetAsync(cacheKey, redisValue, expirationTime);
			}

			return result;
		}

		/// <summary>
		/// 向redis管道发布消息
		/// </summary>
		/// <param name="channel">管道名称</param>
		/// <param name="dbNum">数据库</param>
		///<param name="value">值</param>
		/// <returns></returns>
		public async Task PublishAsync(string channel, object value, int dbNum = -1)
		{
			var redisValue = Serialize(value);
			var transaction = GetTransaction(dbNum);
			//var db = _connectionPool.GetDatabase(dbNum);
			//await db.PublishAsync(channel, redisValue);
			if(transaction == null)
			{
				var db = _connectionPool.GetDatabase(dbNum);
				await db.PublishAsync(channel, redisValue);
			}
			else
			{
				transaction.PublishAsync(channel, redisValue);
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
				var currentTransaction = db.CreateTransaction();
				transactions.Add(dbNum, currentTransaction);
				transaction = currentTransaction;
			}

			return transaction;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="transaction"></param>
		/// <returns></returns>
		public async Task<bool> CommitTransactionAsync(ITransaction transaction)
		{
			return await transaction.ExecuteAsync();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="databaseNumber"></param>
		/// <returns></returns>
		public ITransaction? GetTransaction(int databaseNumber = -1)
		{
			var defaultDbNumber = databaseNumber == -1 ? _redisSettings.DefaultDbNumber : databaseNumber;

			var transaction = transactions
				.Where(p => p.Key == defaultDbNumber)
				.Select(p => p.Value)
				.FirstOrDefault();

			return transaction;
		}

		public async Task<bool> Test(string cacheKey, object value, TimeSpan? expirationTime, int dbNum = -1)
		{
			var redisValue = Serialize(value);
			dbNum = dbNum == -1 ? _redisSettings.DefaultDbNumber : dbNum;
			var db = _connectionPool.GetDatabase(dbNum);
			var currentTransaction = db.CreateTransaction();
			var result = await currentTransaction.StringSetAsync(cacheKey, redisValue, expirationTime);
			var res = await currentTransaction.PublishAsync("testccc", redisValue);
			return true;
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
			switch(type)
			{
				case CommondType.Create:
					if(data == null || expirationTime == null) throw new ArgumentNullException("数据和过期时间不能为空");
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
