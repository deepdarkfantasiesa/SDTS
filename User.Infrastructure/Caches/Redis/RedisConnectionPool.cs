using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Collections.Concurrent;
using System.Net;
using User.Infrastructure.Settings;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace User.Infrastructure.Caches.Redis
{
	/// <summary>
	/// redis连接池
	/// </summary>
	public class RedisConnectionPool
	{
		/// <summary>
		/// 写连接实例集合（激活）
		/// </summary>
		private readonly ConcurrentBag<ConnectionMultiplexer> _activeWriteConnections;

		/// <summary>
		/// 写连接实例集合（失效）
		/// </summary>
		private readonly ConcurrentBag<ConnectionMultiplexer> _failedWriteConnections;

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
			_failedWriteConnections = new ConcurrentBag<ConnectionMultiplexer>();
			_failedReadOnlyConnections = new ConcurrentBag<ConnectionMultiplexer>();
			_activeReadOnlyConnections = new ConcurrentBag<ConnectionMultiplexer>();
			_activeWriteConnections = new ConcurrentBag<ConnectionMultiplexer>();
			_redisSettings = redisSettings.Value;
			_connectionString = _redisSettings.ConnectionString;

			InitConnectionPool();
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
				var writeConnection = GetConnection(_activeWriteConnections);
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

		#region 初始化

		/// <summary>
		/// 重新异步初始化连接池
		/// </summary>
		/// <returns></returns>
		private async Task InitConnectionPoolAsync()
		{
			for(int i = 0; i < _redisSettings.InstanceCount; i++)
			{
				#region 写实例

				var writeConnection = await ConnectionMultiplexer.ConnectAsync(_redisSettings.ConnectionString, options =>
				{
					options.DefaultDatabase = _redisSettings.DefaultDbNumber;
					options.Password = _redisSettings.Password;
					options.AllowAdmin = true;
				});

				writeConnection.ConfigurationChanged += HandleMasterFailoverEvent;
				writeConnection.ConnectionFailed += WriteConnectionFailedEvent;
				writeConnection.ConnectionRestored += WriteConnectionRestoredEvent;

				_activeWriteConnections.Add(writeConnection);

				#endregion

				#region 只读实例

				//获取从库的终结点
				var slaveEndPoints = writeConnection.GetEndPoints().Where(endpoint => writeConnection.GetServer(endpoint).IsReplica).ToArray();

				foreach(var slaveEndPoint in slaveEndPoints)
				{
					var readOnlyConnection = await ConnectionMultiplexer.ConnectAsync(new ConfigurationOptions
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

				var writeConnection = ConnectionMultiplexer.Connect(_redisSettings.ConnectionString, options =>
				{
					options.DefaultDatabase = _redisSettings.DefaultDbNumber;
					options.Password = _redisSettings.Password;
					options.AllowAdmin = true;
				});

				writeConnection.ConfigurationChanged += HandleMasterFailoverEvent;
				writeConnection.ConnectionFailed += WriteConnectionFailedEvent;
				writeConnection.ConnectionRestored += WriteConnectionRestoredEvent;

				_activeWriteConnections.Add(writeConnection);

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
		}

		/// <summary>
		/// 初始化所有连接集合
		/// </summary>
		/// <returns></returns>
		private async Task InitConnectionBag()
		{
			if(_activeReadOnlyConnections.Count != 0)
			{
				foreach(ConnectionMultiplexer activeReadOnlyConnection in _activeReadOnlyConnections)
				{
					activeReadOnlyConnection.ConnectionFailed -= ReadOnlyConnectionFailedEvent;
					activeReadOnlyConnection.ConnectionFailed -= ReadOnlyConnectionRestoredEvent;
					await activeReadOnlyConnection.DisposeAsync();
				}
				_activeReadOnlyConnections.Clear();
			}

			if(_failedReadOnlyConnections.Count != 0)
			{
				foreach(ConnectionMultiplexer failedReadOnlyConnection in _failedReadOnlyConnections)
				{
					failedReadOnlyConnection.ConnectionFailed -= ReadOnlyConnectionFailedEvent;
					failedReadOnlyConnection.ConnectionFailed -= ReadOnlyConnectionRestoredEvent;
					await failedReadOnlyConnection.DisposeAsync();
				}
				_failedReadOnlyConnections.Clear();
			}

			//if(_activeWriteConnections.Count != 0)
			//{
			//	foreach(ConnectionMultiplexer writeConnection in _activeWriteConnections)
			//	{
			//		writeConnection.ConfigurationChanged -= HandleMasterFailoverEvent;
			//		await writeConnection.DisposeAsync();
			//	}
			//	_activeWriteConnections.Clear();
			//}
		}


		#endregion

		#region 连接实例事件

		private async Task ChangeRole(EndPoint masterEndPoint)
		{
			foreach(var failedWriteConnection in _failedWriteConnections)
			{
				failedWriteConnection.ConfigurationChanged -= HandleMasterFailoverEvent;
				var servers = failedWriteConnection.GetServers();
				var failedServers = servers.Where(p => p.IsReplica == false && p.IsConnected == false).ToList();
				//foreach(var failedServer in failedServers)
				//{
				//	await failedServer.ReplicaOfAsync(masterEndPoint);
				//}
				failedWriteConnection.ConnectionFailed += ReadOnlyConnectionFailedEvent;
				failedWriteConnection.ConnectionRestored += ReadOnlyConnectionRestoredEvent;
			}
		}

		private async void HandleMasterFailoverEventV2(object sender, EndPointEventArgs e)
		{
			var connection = sender as ConnectionMultiplexer;
			var currentConnection = connection.GetServers().Where(p => p.EndPoint == e.EndPoint).FirstOrDefault();
			currentConnection.ConfigRewrite();
		}

		private async void HandleMasterFailoverEvent(object sender, EndPointEventArgs e)
		{
			var connection = sender as ConnectionMultiplexer;
			if(connection != null)
			{
				//筛选出主库
				var masterServers = connection.GetServers().Where(p => p.IsReplica == false && p.IsConnected == true).ToList();
				foreach(var masterServer in masterServers)
				{
					//校验主库终结点是否在从库集合中
					var isExist = _activeReadOnlyConnections.Exists(masterServer.EndPoint);
					if(isExist == true)
					{
						//重新初始化
						//await ChangeRole(masterServer.EndPoint);
						await InitConnectionBag();
						await InitConnectionPoolAsync();

						break;
					}

					isExist = _failedReadOnlyConnections.Exists(masterServer.EndPoint);
					if(isExist == true)
					{
						//重新初始化
						//await ChangeRole(masterServer.EndPoint);
						await InitConnectionBag();
						await InitConnectionPoolAsync();

						break;
					}
				}
			}
		}

		/// <summary>
		/// 写库重新连接事件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void WriteConnectionRestoredEvent(object sender, ConnectionFailedEventArgs e)
		{
			var connection = sender as ConnectionMultiplexer;
			if(connection != null)
			{
				var server = connection.GetServers().Where(p => p.EndPoint == e.EndPoint).FirstOrDefault();
				server.ConfigRewrite();
			}
		}

		/// <summary>
		/// 写库断开连接事件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void WriteConnectionFailedEvent(object sender, ConnectionFailedEventArgs e)
		{
			Func<ConnectionMultiplexer, bool> predicate = connection => connection.GetEndPoints().Contains(e.EndPoint);
			TransferConnectionsByCondition(_activeWriteConnections, _failedWriteConnections, predicate);
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

		/// <summary>
		/// 异步初始化连接集合
		/// </summary>
		/// <param name="connections"></param>
		public static async Task InitAsync(this ConcurrentBag<ConnectionMultiplexer> connections)
		{
			if(connections.Count != 0)
			{
				foreach (var connection in connections)
				{
					await connection.DisposeAsync();
				}
				connections.Clear();
			}
		}
	}
}
