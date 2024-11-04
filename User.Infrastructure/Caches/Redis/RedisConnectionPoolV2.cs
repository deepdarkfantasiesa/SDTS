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
			_connections = new ConcurrentBag<ConnectionMultiplexer>();
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
						{_redisSettings.EndPoints[2].Host,_redisSettings.EndPoints[2].Port }
						,{_redisSettings.EndPoints[3].Host,_redisSettings.EndPoints[3].Port },
						{_redisSettings.EndPoints[4].Host,_redisSettings.EndPoints[4].Port },
						{_redisSettings.EndPoints[5].Host,_redisSettings.EndPoints[5].Port },
					},
					Password = _redisSettings.Password,
					DefaultDatabase = _redisSettings.DefaultDbNumber
					//,ServiceName = "local-master"
					//,Proxy=Proxy.Envoyproxy
					
				});

				_connections.Add(connection);

				#endregion

			}
		}

		/// <summary>
		/// 获取redis连接实例
		/// </summary>
		/// <param name="connectionBag">redis连接实例集合</param>
		/// <returns></returns>
		private ConnectionMultiplexer GetConnection(ConcurrentBag<ConnectionMultiplexer> connectionBag)
		{
			var random = new Random();
			var connections = connectionBag.ToList();
			var randomIndex = random.Next(connections.Count);
			var connection = connections[randomIndex];
			return connection;
		}


		/// <summary>
		/// 获取从redis的数据库
		/// </summary>
		/// <param name="useReplica">是否使用从库</param>
		/// <param name="dbNum">数据库编号</param>
		/// <returns></returns>
		public IDatabase GetDatabase(bool useReplica = false, int dbNum = 0)
		{
			var random = new Random();
			if(useReplica == true && _connections.Count > 0)
			{
				var readOnlyConnection = GetConnection(_connections);

				var servers = readOnlyConnection.GetServers().Where(p => p.IsReplica == true).ToList();
				//var randomIndex = random.Next(servers.Count);
				//var connection = servers[randomIndex].Multiplexer;
				//return connection.GetDatabase(dbNum);

				return readOnlyConnection.GetDatabase(dbNum);
			}
			else
			{
				var writeConnection = GetConnection(_connections);
				return writeConnection.GetDatabase(dbNum);
			}
		}

	}
}
