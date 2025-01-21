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
		/// redis配置类
		/// </summary>
		private RedisSettings _redisSettings;

		/// <summary>
		/// 连接实例集合
		/// </summary>
		private readonly ConcurrentBag<ConnectionMultiplexer> _connections;

		/// <summary>
		/// redis连接池
		/// </summary>
		/// <param name="redisSettings">redis配置模块</param>
		public RedisConnectionPool(IOptionsMonitor<RedisSettings> redisSettings)
		{
			_redisSettings = redisSettings.CurrentValue;
			redisSettings.OnChange(OnChangeSettings);
			_connections = new ConcurrentBag<ConnectionMultiplexer>();
			InitConnectionPool();
		}

		/// <summary>
		/// 配置类监听函数
		/// </summary>
		/// <param name="redisSettings"></param>
		private async void OnChangeSettings(RedisSettings redisSettings)
		{
			if (_redisSettings.Equals(redisSettings)) return;
			Console.WriteLine("RedisSettingsChanged");
			_redisSettings = redisSettings;
			InitConnectionPool();
		}

		/// <summary>
		/// 重新初始化连接池
		/// </summary>
		/// <returns></returns>
		private void InitConnectionPool()
		{
			if(_connections.Count > 0)
			{
				foreach (var connection in _connections)
				{
					connection.Dispose();
				}
				_connections.Clear();
			}

			for(int i = 0; i < _redisSettings.InstanceCount; i++)
			{
				var connection = ConnectionMultiplexer.Connect(new ConfigurationOptions
				{
					AllowAdmin = true,
					EndPoints = {
						{_redisSettings.EndPoints[0].Host,_redisSettings.EndPoints[0].Port },
						{_redisSettings.EndPoints[1].Host,_redisSettings.EndPoints[1].Port },
						{_redisSettings.EndPoints[2].Host,_redisSettings.EndPoints[2].Port },
						{_redisSettings.EndPoints[3].Host,_redisSettings.EndPoints[3].Port },
						{_redisSettings.EndPoints[4].Host,_redisSettings.EndPoints[4].Port },
						{_redisSettings.EndPoints[5].Host,_redisSettings.EndPoints[5].Port },
					},
					Password = _redisSettings.Password,
					DefaultDatabase = _redisSettings.DefaultDbNumber
					//,ServiceName = "local-master"
					//,Proxy=Proxy.Envoyproxy
				});

				_connections.Add(connection);
			}
		}

		/// <summary>
		/// 获取redis连接实例
		/// </summary>
		/// <param name="connectionBag">redis连接实例集合</param>
		/// <returns></returns>
		public ConnectionMultiplexer GetConnection()
		{
			var random = new Random();
			var connections = _connections.ToList();
			var randomIndex = random.Next(connections.Count);
			var connection = connections[randomIndex];
			return connection;
		}

		/// <summary>
		/// 获取redis的数据库
		/// </summary>
		/// <param name="dbNum">数据库编号</param>
		/// <returns></returns>
		public IDatabase GetDatabase(int dbNum = 0)
		{
			if(_connections.Count == 1)
			{
				var connection = _connections.First();
				return connection.GetDatabase(dbNum);
			}
			else if(_connections.Count >1) 
			{
				var connection = GetConnection();
				return connection.GetDatabase(dbNum);
			}

			throw new Exception("redis连接实例为空");
		}

		/// <summary>
		/// 获取所有redis连接实例
		/// </summary>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public IEnumerable<ConnectionMultiplexer> GetAllConnections()
		{
			if (_connections.Count == 0)
				throw new Exception("连接池中没有连接实例");
			return _connections.AsEnumerable();
		}
	}
}
