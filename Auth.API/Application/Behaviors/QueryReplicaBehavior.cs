using Auth.Infrastructure.Caches;
using Infrastructure.Core;
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
    public class QueryReplicaBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IQueryReplica<TResponse>
    {
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
        public QueryReplicaBehavior(ICacheImpl cache, IRegistryService serviceCenter, IQueryDbContext queryDbContext)
        {
            _cache = cache;
            _serviceCenter = serviceCenter;
            _queryDbContext = queryDbContext;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!request.UseReplica)
                return await next();

            //从缓存中获取数据库实例的信息
            var cacheResult = await _cache.GetHashAsync<IEnumerable<RelationalDatabaseModel>>(CacheKeyPrefix.PgSqlsConfig, CacheLevel.Local);

            var rdbs = cacheResult.IsHit == true ? cacheResult.Value : null;

            if (rdbs == null || rdbs.Count() == 0)
            {
                rdbs = await _serviceCenter.DiscoverRDB("pgsql");
            }

            var replicaRdbs = rdbs.Where(p => p.Tag.Contains("replica")).ToList();
            if (replicaRdbs != null && replicaRdbs.Count > 0)
            {
                var radom = new Random();
                var replicaRdb = replicaRdbs[radom.Next(replicaRdbs.Count)];
                _queryDbContext.ConnectionString = $"Host={replicaRdb.Address}:{replicaRdb.Port};Database=postgres;Username=postgres;Password=postgres";
            }
            else
            {
                Console.WriteLine("设置从库连接字符串失败");
            }

            var response = await next();

            return response;
        }
    }
}
