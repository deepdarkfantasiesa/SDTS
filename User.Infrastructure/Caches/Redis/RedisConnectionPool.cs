using Microsoft.Extensions.Caching.Memory;
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
		/// 内存缓存
		/// </summary>
		private readonly IMemoryCache _memoryCache;

		/// <summary>
		/// redis连接池
		/// </summary>
		/// <param name="redisSettings">redis配置模块</param>
		/// <param name="memoryCache">内存缓存</param>
		public RedisConnectionPool(IOptionsMonitor<RedisSettings> redisSettings, IMemoryCache memoryCache)
		{
			_memoryCache = memoryCache;
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
			if(_redisSettings.Equals(redisSettings)) return;
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
				foreach(var connection in _connections)
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
					DefaultDatabase = _redisSettings.DefaultDbNumber,
					HeartbeatConsistencyChecks = true,
					//,ServiceName = "local-master"
					//,Proxy=Proxy.Envoyproxy
				});
				//订阅连接失败后触发的事件
				connection.ConnectionFailed += ConnectionFailed;

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
		public IDatabase GetDatabase(int? dbNum)
		{
			if(dbNum == null) dbNum = _redisSettings.DefaultDbNumber;
			if(_connections.Count == 1)
			{
				var connection = _connections.First();
				return connection.GetDatabase(dbNum.Value);
			}
			else if(_connections.Count > 1)
			{
				var connection = GetConnection();
				return connection.GetDatabase(dbNum.Value);
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
			if(_connections.Count == 0)
				throw new Exception("连接池中没有连接实例");
			return _connections.AsEnumerable();
		}

		#region 事件

		/// <summary>
		/// 连接失败时触发的事件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void ConnectionFailed(object sender, ConnectionFailedEventArgs e)
		{
			//清除内存缓存
			if(_memoryCache is MemoryCache cache)
			{
				cache.Clear();
				//Console.WriteLine("clear all InMemoryCache");
			}
		}

		#endregion
	}
}
