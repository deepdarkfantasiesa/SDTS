using StackExchange.Redis;
using System.Collections.Concurrent;

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
		private readonly ConcurrentBag<IConnectionMultiplexer> _connections;

		/// <summary>
		/// 最大实例数
		/// </summary>
		private readonly int _maxSize;

		/// <summary>
		/// 连接字符串
		/// </summary>
		private readonly string _connectionString;

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
		public RedisConnectionPool(string connectionString, int maxSize)
		{
			_connections = new ConcurrentBag<IConnectionMultiplexer>();
			_maxSize = maxSize;
			_connectionString = connectionString;

			// 初始化连接池
			for (int i = 0; i < maxSize; i++)
			{
				_connections.Add(ConnectionMultiplexer.Connect(connectionString));
			}
		}

		/// <summary>
		/// 获取连接实例
		/// </summary>
		/// <returns></returns>
		public PooledConnection GetConnection()
		{
			//尝试从redis连接实例集合中获取实例
			if (_connections.TryTake(out var connection))
			{
				return new PooledConnection(this, connection);
			}

			// 如果没有可用连接，则创建新连接
			connection = ConnectionMultiplexer.Connect(_connectionString);
			return new PooledConnection(this, connection);
		}

		/// <summary>
		/// 回收连接实例
		/// </summary>
		/// <param name="connection">redis连接实例</param>
		private void ReturnConnection(IConnectionMultiplexer connection)
		{
			_connections.Add(connection);
		}

		/// <summary>
		/// 池化连接类
		/// </summary>
		public class PooledConnection : IDisposable
		{
			/// <summary>
			/// redis连接池
			/// </summary>
			private readonly RedisConnectionPool _pool;

			/// <summary>
			/// redis连接实例
			/// </summary>
			private IConnectionMultiplexer _connection;

			/// <summary>
			/// 池化连接类
			/// </summary>
			/// <param name="pool">redis连接池</param>
			/// <param name="connection">redis连接实例</param>
			public PooledConnection(RedisConnectionPool pool, IConnectionMultiplexer connection)
			{
				_pool = pool;
				_connection = connection;
			}

			/// <summary>
			/// redis连接实例
			/// </summary>
			public IConnectionMultiplexer Connection { get => _connection; }

			/// <summary>
			/// 释放实例
			/// </summary>
			public void Dispose()
			{
				//如果连接池没有达到上限则回收连接实例
				if (_connection != null && _pool._connections.Count() < _pool._maxSize) 
				{
					_pool.ReturnConnection(_connection);
				}
				else
				{
					//否则释放连接实例
					_connection.Dispose();
                    Console.WriteLine($"ConnectionMultiplexer实例{_connection.GetHashCode()}已被释放");
                    _connection = null;
				}
			}
		}
	}
}
