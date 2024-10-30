using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Collections.Concurrent;
using User.Infrastructure.Settings;

namespace User.Infrastructure.Caches.Redis
{
	/// <summary>
	/// redis连接池
	/// </summary>
	public class RedisConnectionPool
	{
		/// <summary>
		/// redis连接实例集合
		/// </summary>
		private readonly ConcurrentBag<ConnectionMultiplexer> _connections;

		/// <summary>
		/// 只读
		/// </summary>
		private readonly ConcurrentBag<ConnectionMultiplexer> _readOnlyConnections;

		/// <summary>
		/// 连接字符串
		/// </summary>
		private readonly string _connectionString;

		/// <summary>
		/// redis配置类
		/// </summary>
		private readonly RedisSettings _redisSettings;

		/// <summary>
		/// 最新连接实例数
		/// </summary>
		public int CurrentConnectionCount
		{
			get
			{
				return _connections.Count;
			}
		}

		/// <summary>
		/// redis连接池
		/// </summary>
		/// <param name="connectionString">连接字符串</param>
		/// <param name="maxSize">最大实例数</param>
		public RedisConnectionPool(IOptions<RedisSettings> redisSettings)
		{
			_readOnlyConnections = new ConcurrentBag<ConnectionMultiplexer>();
			_connections = new ConcurrentBag<ConnectionMultiplexer>();
			_redisSettings = redisSettings.Value;
			_connectionString = _redisSettings.ConnectionString;

			//实例化
			for (int i = 0; i < _redisSettings.InstanceCount; i++)
			{
				#region 可读可写的实例

				var connection = ConnectionMultiplexer.Connect(_redisSettings.ConnectionString, options =>
				{
					options.DefaultDatabase = _redisSettings.DefaultDbNumber;
					options.Password = _redisSettings.Password;
				});

				connection.ConnectionFailed += (sender, arges) =>
				{
					Console.WriteLine($"{arges.EndPoint} is ConnectionFailed");
					foreach (var readOnlyConnection in _readOnlyConnections)
					{
						readOnlyConnection.DisposeAsync();
					}
					_readOnlyConnections.Clear();
				};
				_connections.Add(connection);

				#endregion

				#region 只读实例

				var slaveEndPoints = connection.GetEndPoints().Where(endpoint => connection.GetServer(endpoint).IsReplica).ToArray();
				// 初始化连接池
				foreach (var slaveEndPoint in slaveEndPoints)
				{
					var readOnlyConnection = ConnectionMultiplexer.Connect(new ConfigurationOptions
					{
						EndPoints = { slaveEndPoint },
						AllowAdmin = true,
						Password = _redisSettings.Password,
						DefaultDatabase = _redisSettings.DefaultDbNumber
					});

					_readOnlyConnections.Add(readOnlyConnection);
				}

				#endregion
			}
		}

		/// <summary>
		/// 获取从redis的数据库
		/// </summary>
		/// <param name="dbNum">数据库编号</param>
		/// <returns></returns>
		public IDatabase GetReadOnlyDatabase(int dbNum = 0)
		{
			var readOnlyConnectionList = _readOnlyConnections.ToList();
			var random = new Random();
			var randomIndex = random.Next(readOnlyConnectionList.Count);
			var readOnlyConnection = readOnlyConnectionList[randomIndex];
			return readOnlyConnection.GetDatabase(dbNum);
		}
	}
}
