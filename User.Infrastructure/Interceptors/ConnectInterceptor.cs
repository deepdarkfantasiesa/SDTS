using Infrastructure.Core;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Service.Framework.Models;
using Service.Framework.ServiceRegistry;
using System.Data.Common;
using User.Infrastructure.Caches;

namespace User.Infrastructure.Interceptors
{
	/// <summary>
	/// 连接拦截器
	/// </summary>
	public class ConnectInterceptor : DbConnectionInterceptor
	{
		/// <summary>
		/// 缓存实现类
		/// </summary>
		private readonly ICacheImpl _cache;

		/// <summary>
		/// 服务中心
		/// </summary>
		private readonly IRegistryService _serviceCenter;

		/// <summary>
		/// 连接拦截器
		/// </summary>
		/// <param name="cache">缓存实现类</param>
		/// <param name="serviceCenter">服务中心</param>
		public ConnectInterceptor(ICacheImpl cache, IRegistryService serviceCenter)
		{
			_cache = cache;
			_serviceCenter = serviceCenter;
		}

		/// <summary>
		/// 连接创建前（池化注册不会每次获取时运行）
		/// </summary>
		/// <param name="eventData"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public override InterceptionResult<DbConnection> ConnectionCreating(ConnectionCreatingEventData eventData, InterceptionResult<DbConnection> result)
		{
			//Console.WriteLine("this is ConnectionCreating");

			#region 预留的写法（池化注册时不适用，需要在ConnectionOpeningAsync写）

			////TODO:从心跳中心获取从库的连接字符串

			//// 创建新的 DbConnection 对象
			//var newConnection = new MySqlConnection("连接字符串);

			//// 返回新的 DbConnection 对象
			//return InterceptionResult<DbConnection>.SuppressWithResult(newConnection);

			#endregion

			return base.ConnectionCreating(eventData, result);
		}

		/// <summary>
		/// 连接开启前，在连接创建前之后
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="eventData"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public override InterceptionResult ConnectionOpening(DbConnection connection, ConnectionEventData eventData, InterceptionResult result)
		{
			//Console.WriteLine("this is ConnectionOpening");
			return base.ConnectionOpening(connection, eventData, result);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="eventData"></param>
		/// <param name="result"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public override async ValueTask<InterceptionResult> ConnectionOpeningAsync(DbConnection connection, ConnectionEventData eventData, InterceptionResult result, CancellationToken cancellationToken = default)
		{
			var tags = new CacheTag[]
			{
				CacheTag.RelationDatabaseConfig,
				CacheTag.HealthCheck,
				CacheTag.Background
			};
			//从缓存中获取数据库实例的信息
			var cacheResult = await _cache.GetStringAsync<List<RelationalDatabaseModel>>(CacheKeyPrefix.PgSqlsConfig, tags, preferLocal: true);

			var rdbCaches = cacheResult.IsHit == true ? cacheResult.Value : null;


            RelationalDatabaseModel replicaConfig = null;
			if (rdbCaches != null && rdbCaches.Count > 0)
			{
				replicaConfig = TryGetReplicaConfig(rdbCaches);
			}
			else
			{
				//从服务发现中心获取
				var rdbConfigs = await _serviceCenter.DiscoverRDB("pgsql");

				if (rdbConfigs != null && rdbConfigs.ToList().Count > 0) 
					replicaConfig = TryGetReplicaConfig(rdbConfigs);

				if (replicaConfig == null)
				{
					//TODO：如果服务发现中心还是没有则手动实例化
					replicaConfig = new RelationalDatabaseModel
					{

					};
				}
			}

			if (!connection.ConnectionString.Contains($"{replicaConfig.Address}") || !connection.ConnectionString.Contains($"{replicaConfig.Port}"))
				connection.ConnectionString = $"Host={replicaConfig.Address}:{replicaConfig.Port};Database=postgres;Username=postgres;Password=postgres";

			return await base.ConnectionOpeningAsync(connection, eventData, result, cancellationToken);
		}

		/// <summary>
		/// 尝试获取从库的配置
		/// </summary>
		/// <param name="rdbs"></param>
		/// <returns></returns>
		private RelationalDatabaseModel TryGetReplicaConfig(IEnumerable<RelationalDatabaseModel> rdbs)
		{
			var radom = new Random();

			var replicaRdbs = rdbs.Where(p => p.Tag.Contains("replica")).ToList();
			if (replicaRdbs != null && replicaRdbs.Count > 0)
			{
				var replicaRdb = replicaRdbs[radom.Next(replicaRdbs.Count)];
				return replicaRdb;
			}

			var masterRdbs = rdbs.Where(p => p.Tag.Contains("master")).ToList();
			if (masterRdbs != null && masterRdbs.Count > 0) 
			{
				var masterRdb = masterRdbs[radom.Next(masterRdbs.Count)];
				return masterRdb;
			}

			return null;
		}
	}
}
