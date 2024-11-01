using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Collections.Concurrent;
using User.Infrastructure.Settings;

namespace User.Infrastructure.Caches.Redis
{
	public class RedisConnectionPoolV2
	{
        private readonly RedisSettings _redisSettings;

		private readonly ConcurrentBag<ConnectionMultiplexer> _connections;

		public RedisConnectionPoolV2(IOptions<RedisSettings> options)
        {
            _redisSettings = options.Value;
			InitConnectionPool();
        }

		/// <summary>
		/// 重新初始化连接池
		/// </summary>
		/// <returns></returns>
		private void InitConnectionPool()
		{
			for(int i = 0; i < _redisSettings.InstanceCount; i++)
			{
				#region 写实例

				var connection = ConnectionMultiplexer.Connect(new ConfigurationOptions
				{
					AllowAdmin = true,
					EndPoints = {
						{_redisSettings.EndPoints[0].Host,_redisSettings.EndPoints[0].Port },
						{_redisSettings.EndPoints[1].Host,_redisSettings.EndPoints[1].Port },
						{_redisSettings.EndPoints[2].Host,_redisSettings.EndPoints[2].Port },
					},
					Password = _redisSettings.Password,
					DefaultDatabase=_redisSettings.DefaultDbNumber,
					ServiceName= "local-master"
				});

				_connections.Add(connection);

				#endregion

			}
		}
	}
}
