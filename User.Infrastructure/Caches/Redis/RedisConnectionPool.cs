using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Collections.Concurrent;
using System.Net;
using User.Infrastructure.Settings;

namespace User.Infrastructure.Caches.Redis
{
	/// <summary>
	/// redis连接池
	/// </summary>
	public class RedisConnectionPool
	{
		/// <summary>
		/// 写连接实例集合
		/// </summary>
		private readonly ConcurrentBag<ConnectionMultiplexer> _writeConnections;

		/// <summary>
		/// 只读连接实例集合（激活）
		/// </summary>
		private readonly ConcurrentBag<ConnectionMultiplexer> _activeReadOnlyConnections;

		/// <summary>
		/// 只读连接实例集合（失效）
		/// </summary>
		private readonly ConcurrentBag<ConnectionMultiplexer> _failedReadOnlyConnections;

		/// <summary>
		/// 连接字符串
		/// </summary>
		private readonly string _connectionString;

		/// <summary>
		/// redis配置类
		/// </summary>
		private readonly RedisSettings _redisSettings;

		/// <summary>
		/// redis连接池
		/// </summary>
		/// <param name="connectionString">连接字符串</param>
		/// <param name="maxSize">最大实例数</param>
		public RedisConnectionPool(IOptions<RedisSettings> redisSettings)
		{
			_failedReadOnlyConnections = new ConcurrentBag<ConnectionMultiplexer>();
			_activeReadOnlyConnections = new ConcurrentBag<ConnectionMultiplexer>();
			_writeConnections = new ConcurrentBag<ConnectionMultiplexer>();
			_redisSettings = redisSettings.Value;
			_connectionString = _redisSettings.ConnectionString;

			for(int i = 0; i < _redisSettings.InstanceCount; i++)
			{
				#region 写实例

				var writeConnection = ConnectionMultiplexer.Connect(_redisSettings.ConnectionString, options =>
				{
					options.DefaultDatabase = _redisSettings.DefaultDbNumber;
					options.Password = _redisSettings.Password;
				});

				writeConnection.ConfigurationChanged += HandleMasterFailover;

				_writeConnections.Add(writeConnection);

				#endregion

				#region 只读实例

				//获取从库的终结点
				var slaveEndPoints = writeConnection.GetEndPoints().Where(endpoint => writeConnection.GetServer(endpoint).IsReplica).ToArray();

				foreach(var slaveEndPoint in slaveEndPoints)
				{
					var readOnlyConnection = ConnectionMultiplexer.Connect(new ConfigurationOptions
					{
						EndPoints = { slaveEndPoint },
						AllowAdmin = true,
						Password = _redisSettings.Password,
						DefaultDatabase = _redisSettings.DefaultDbNumber
					});

					//订阅从库断开连接事件
					readOnlyConnection.ConnectionFailed += ReadOnlyConnectionFailedEvent;

					//订阅从库重新连接事件
					readOnlyConnection.ConnectionRestored += ReadOnlyConnectionRestoredEvent;

					_activeReadOnlyConnections.Add(readOnlyConnection);
				}
				#endregion
			}

			#region
			////实例化
			//for (int i = 0; i < _redisSettings.InstanceCount; i++)
			//{
			//	#region 可读可写的实例

			//	var connection = ConnectionMultiplexer.Connect(_redisSettings.ConnectionString, options =>
			//	{
			//		options.DefaultDatabase = _redisSettings.DefaultDbNumber;
			//		options.Password = _redisSettings.Password;
			//	});

			//	connection.ConnectionFailed += (sender, arges) =>
			//	{
			//		Console.WriteLine($"{arges.EndPoint} is ConnectionFailed");
			//		foreach (var readOnlyConnection in _readOnlyConnections)
			//		{
			//			readOnlyConnection.DisposeAsync();
			//		}
			//		_readOnlyConnections.Clear();
			//	};
			//	_writeConnections.Add(connection);

			//	#endregion

			//	#region 只读实例

			//	var slaveEndPoints = connection.GetEndPoints().Where(endpoint => connection.GetServer(endpoint).IsReplica).ToArray();
			//	// 初始化连接池
			//	foreach (var slaveEndPoint in slaveEndPoints)
			//	{
			//		var readOnlyConnection = ConnectionMultiplexer.Connect(new ConfigurationOptions
			//		{
			//			EndPoints = { slaveEndPoint },
			//			AllowAdmin = true,
			//			Password = _redisSettings.Password,
			//			DefaultDatabase = _redisSettings.DefaultDbNumber
			//		});

			//		_readOnlyConnections.Add(readOnlyConnection);
			//	}

			//	#endregion
			//}
			#endregion
		}

		#region 对外操作

		/// <summary>
		/// 获取从redis的数据库
		/// </summary>
		/// <param name="useReplica">是否使用从库</param>
		/// <param name="dbNum">数据库编号</param>
		/// <returns></returns>
		public IDatabase GetDatabase(bool useReplica = false, int dbNum = 0)
		{
			var random = new Random();
			if(useReplica == true && _activeReadOnlyConnections.Count > 0)
			{
				var readOnlyConnection = GetConnection(_activeReadOnlyConnections);
				return readOnlyConnection.GetDatabase(dbNum);
			}
			else
			{
				var writeConnection = GetConnection(_writeConnections);
				return writeConnection.GetDatabase(dbNum);
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

		#endregion

		#region 连接实例事件

		private async void HandleMasterFailover(object sender, EndPointEventArgs e)
		{
			var connection = sender as ConnectionMultiplexer;
			if(connection != null)
			{
				//筛选出主库
				var masterServers = connection.GetServers().Where(p => p.IsReplica == false).ToList();
				foreach(var masterServer in masterServers)
				{
					//校验主库终结点是否在从库集合中
					var isExist = _activeReadOnlyConnections.Exists(masterServer.EndPoint);
					if(isExist == true)
					{
						//重新初始化
						
						break;
					}

					isExist = _failedReadOnlyConnections.Exists(masterServer.EndPoint);
					if(isExist == true)
					{
						//重新初始化
						break;
					}
					//await masterServer.ReplicaOfAsync(masterServer.EndPoint);
				}
			}
		}

		/// <summary>
		/// 从库重新连接事件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ReadOnlyConnectionRestoredEvent(object sender, ConnectionFailedEventArgs e)
		{
			if(e.ConnectionType == ConnectionType.Interactive)
			{
				var activeConnection = sender as ConnectionMultiplexer;
				if(activeConnection != null)
				{
					Console.WriteLine($"{e.ConnectionType} {e.EndPoint} is ConnectionRestored at {DateTime.Now}");
					Func<ConnectionMultiplexer, bool> predicate = connection => connection.GetEndPoints().Contains(e.EndPoint);
					TransferConnectionsByCondition(_failedReadOnlyConnections, _activeReadOnlyConnections, predicate);
				}
			}

			if(e.ConnectionType == ConnectionType.Subscription)
			{

			}
		}

		/// <summary>
		/// 从库断开连接事件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ReadOnlyConnectionFailedEvent(object sender, ConnectionFailedEventArgs e)
		{
			if(e.ConnectionType == ConnectionType.Interactive)
			{
				// 移除连接失败的实例
				var failedConnection = sender as ConnectionMultiplexer;
				if(failedConnection != null)
				{
					Console.WriteLine($"{e.ConnectionType} {e.EndPoint} is ConnectionFailed at {DateTime.Now}");
					Func<ConnectionMultiplexer, bool> predicate = connection => connection.GetEndPoints().Contains(e.EndPoint);
					TransferConnectionsByCondition(_activeReadOnlyConnections, _failedReadOnlyConnections, predicate);
				}
			}

			if(e.ConnectionType == ConnectionType.Subscription)
			{

			}
		}

		/// <summary>
		/// 在集合之间转移实例
		/// </summary>
		/// <param name="sourceConnectionBag">源集合</param>
		/// <param name="targetConnectionBag">目标集合</param>
		/// <param name="predicate">表达式</param>
		private void TransferConnectionsByCondition(ConcurrentBag<ConnectionMultiplexer> sourceConnectionBag, ConcurrentBag<ConnectionMultiplexer> targetConnectionBag, Func<ConnectionMultiplexer, bool> predicate)
		{
			var temporaryBag = new ConcurrentBag<ConnectionMultiplexer>();
			foreach(var sourceConnection in sourceConnectionBag)
			{
				if(!predicate(sourceConnection))
				{
					temporaryBag.Add(sourceConnection);
				}
				else
				{
					targetConnectionBag.Add(sourceConnection);
				}
			}

			while(!sourceConnectionBag.IsEmpty)
			{
				sourceConnectionBag.TryTake(out _);
			}

			foreach(var connection in temporaryBag)
			{
				sourceConnectionBag.Add(connection);
			}
		}

		#endregion

	}

	public static class ConcurrentBagExtensions
	{
		public static bool Exists(this ConcurrentBag<ConnectionMultiplexer> connections, EndPoint endPoint)
		{
			foreach(var connection in connections)
			{
				if(connection.GetEndPoints().Contains(endPoint) == true)
				{
					return true;
				}
			}
			return false;
		}
	}
}
