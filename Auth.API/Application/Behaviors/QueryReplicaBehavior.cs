using Auth.Infrastructure.Caches;
using Infrastructure.Core.Cache;
using Infrastructure.Core.DatabaseContext;
using Infrastructure.Core.Extension;
using Infrastructure.Core.Query;
using MediatR;
using Service.Framework.Models;
using Service.Framework.ServiceRegistry;

namespace Auth.API.Application.Behaviors
{
    /// <summary>
    /// query从库管道行为
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class QueryReplicaBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IQueryReplica
    {
        private readonly ILogger<QueryReplicaBehavior<TRequest, TResponse>> _logger;

        /// <summary>
        /// 缓存
        /// </summary>
        private readonly ICacheImpl _cache;

        /// <summary>
        /// 服务发现中心
        /// </summary>
        private readonly IRegistryService _serviceCenter;

        /// <summary>
        /// query上下文
        /// </summary>
        private readonly IQueryDbContext _queryDbContext;

        /// <summary>
        /// query从库管道行为
        /// </summary>
        /// <param name="cache">缓存</param>
        /// <param name="serviceCenter">服务发现中心</param>
        /// <param name="queryDbContext">query上下文</param>
        public QueryReplicaBehavior(ICacheImpl cache, IRegistryService serviceCenter, IQueryDbContext queryDbContext, ILogger<QueryReplicaBehavior<TRequest, TResponse>> logger)
        {
            _cache = cache;
            _serviceCenter = serviceCenter;
            _queryDbContext = queryDbContext;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var response = default(TResponse);

            if (!request.UseReplica)
            {
                _logger.LogInformation("对主库进行Query {QueryName} ({@Query})", request.GetGenericTypeName(), request);
                response = await next();
                _logger.LogInformation("Query完成， {QueryName} ({@Response})", request.GetGenericTypeName(), response);
                return response;
            }

            _logger.LogInformation("对从库进行Query {QueryName} ({@Query})", request.GetGenericTypeName(), request);

            _logger.LogInformation("检索缓存中的从库连接字符串 {CacheKey}", CacheKeyPrefix.PgSqlsConfig);
            //从缓存中获取数据库实例的信息
            var cacheResult = await _cache.GetHashAsync<IEnumerable<RelationalDatabaseModel>>(CacheKeyPrefix.PgSqlsConfig, CacheLevel.Local);
            _logger.LogInformation("检索缓存完成 {CacheKey}", CacheKeyPrefix.PgSqlsConfig);
            var rdbs = cacheResult.IsHit == true ? cacheResult.Value : null;

            if (rdbs == null || rdbs.Count() == 0)
            {
                _logger.LogInformation("未命中缓存 {CacheKey},向服务发现中心获取从库连接字符串", CacheKeyPrefix.PgSqlsConfig);
                rdbs = await _serviceCenter.DiscoverRDB("pgsql");
                _logger.LogInformation("检索服务发现中心完成 {@RelationalDatabase}", rdbs);
            }

            var replicaRdbs = rdbs.Where(p => p.Tag.Contains("replica")).ToList();
            if (replicaRdbs != null && replicaRdbs.Count > 0)
            {
                var radom = new Random();
                var replicaRdb = replicaRdbs[radom.Next(replicaRdbs.Count)];
                _queryDbContext.ConnectionString = $"Host={replicaRdb.Address}:{replicaRdb.Port};Database=postgres;Username=postgres;Password=postgres";
                _logger.LogInformation("设置从库连接字符串成功 {@ReplicaDatabase}", replicaRdb);
            }
            else
            {
                _logger.LogInformation("设置从库连接字符串失败");
            }

            _logger.LogInformation("开始QueryDB");
            response = await next();
            _logger.LogInformation("QueryDB结束 {@Response}", response);

            return response;
        }
    }
}
