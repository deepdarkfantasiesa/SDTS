using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using User.Infrastructure.Settings;

namespace User.Infrastructure.Caches.Redis
{
	/// <summary>
	/// redis上下文
	/// </summary>
	public class RedisContext
	{
		/// <summary>
		/// redis连接池
		/// </summary>
		private readonly RedisConnectionPool _connectionPool;

		/// <summary>
		/// redis配置类
		/// </summary>
		private readonly RedisSettings _redisSettings;

		/// <summary>
		/// redis上下文
		/// </summary>
		/// <param name="connectionPool">redis连接池</param>
		/// <param name="redisSettings">redis配置类</param>
		public RedisContext(RedisConnectionPool connectionPool, IOptions<RedisSettings> redisSettings)
		{
			_connectionPool = connectionPool;
			_redisSettings = redisSettings.Value;
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
			using (var pooledConnection = await _connectionPool.GetConnectionAsync())
			{
				var connection = pooledConnection.Connection;
				var db = connection.GetDatabase(databaseNumber != null ? databaseNumber.Value : _redisSettings.DefaultDbNumber);
				var cacheData = await db.StringGetAsync(cacheKey);
				return Deserialize<T>(cacheData);
			}
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
