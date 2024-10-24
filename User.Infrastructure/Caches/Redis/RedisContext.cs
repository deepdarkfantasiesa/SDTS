using Microsoft.Extensions.Options;
using System.Text.Json;
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

		private readonly RedisSettings _redisSettings;

		/// <summary>
		/// redis上下文
		/// </summary>
		/// <param name="connectionPool">redis连接池</param>
		public RedisContext(RedisConnectionPool connectionPool, IOptions<RedisSettings> redisSettings)
		{
			_connectionPool = connectionPool;
			_redisSettings = redisSettings.Value;
		}

		public async Task<T> GetStringAsync<T>(string key, int databaseNumber = 0)
		{
			using (var pooledConnection = await _connectionPool.GetConnectionAsync())
			{
				var connection = pooledConnection.Connection;
				var db = connection.GetDatabase();
				var cacheData = await db.StringGetAsync(key);
				var seriData = JsonSerializer.Deserialize<T>(cacheData);
				return seriData;
				//if (cacheData != null)
				//{

				//}
				//else
				//{
				//	return default(T);
				//}
			}
		}
	}
}
