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
		/// redis上下文
		/// </summary>
		/// <param name="connectionPool">redis连接池</param>
		/// <param name="redisSettings">redis配置类</param>
		public RedisContext(RedisConnectionPool connectionPool, IOptionsMonitor<RedisSettings> redisSettings)
		{
			_connectionPool = connectionPool;
			redisSettings.OnChange(RedisSettingChange);
			_redisSettings = redisSettings.CurrentValue;
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
		/// <returns></returns>
		public async Task<T> GetStringAsync<T>(string cacheKey, int? databaseNumber = null)
		{
			if (databaseNumber == null)
				databaseNumber = _redisSettings.DefaultDbNumber;

			var db = _connectionPool.GetDatabase(databaseNumber.Value);
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
		/// <returns></returns>
		public async Task<bool> SetStringAsync(string cacheKey, object value, TimeSpan? expirationTime = null, int? databaseNumber = null)
		{
			if (expirationTime == null)
				expirationTime = TimeSpan.FromSeconds(30);
			if (databaseNumber == null)
				databaseNumber = _redisSettings.DefaultDbNumber;

			var db = _connectionPool.GetDatabase(databaseNumber.Value);
			var redisValue = System.Text.Json.JsonSerializer.Serialize(value);
			return await db.StringSetAsync(cacheKey, redisValue, expirationTime);
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

		#endregion
	}
}
