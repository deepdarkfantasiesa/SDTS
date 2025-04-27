using Infrastructure.Core;
using Infrastructure.Core.Query;
using MediatR;
using User.Infrastructure.Caches;
using User.Infrastructure.Caches.Models.SyncMemoryCacheCommds;

namespace User.API.Application.Behaviors
{
    /// <summary>
    /// Query缓存管道行为
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class QueryCacheBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IQueryCache<TResponse>
    {
        /// <summary>
        /// 缓存上下文
        /// </summary>
        private readonly ICacheImpl _cacheImpl;

        /// <summary>
        /// Query管道行为
        /// </summary>
        /// <param name="cacheImpl">缓存上下文</param>
        public QueryCacheBehavior(ICacheImpl cacheImpl)
        {
            _cacheImpl = cacheImpl;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            //判断是否使用缓存
            if (request.PreferCacheLevel == QueryCacheLevel.None)
                return await next();

            //判断缓存优先级
            var cacheLevel = request.PreferCacheLevel == QueryCacheLevel.Local ? CacheLevel.Local : CacheLevel.Distributed;

            QueryCacheResult<TResponse> cache = null;
            //查询缓存
            cache = await _cacheImpl.GetHashAsync<TResponse>(request.CacheKey, cacheLevel);

            if (cache.IsHit)
            {
                //若命中缓存则直接返回缓存结果
                return cache.Value;
            }

            //运行数据库查询逻辑
            var response = await next();

            #region 插入缓存

            //开启redis事务
            var transaction = _cacheImpl.BeginTransaction();

            //写入缓存
            if (request.Tags == null || request.Tags.Count() == 0)
            {
                await _cacheImpl.SetHashAsync(request.CacheKey, response, request.CacheDuration);
            }
            else
            {
                await _cacheImpl.SetHashAsync(request.CacheKey, response, tags: request.Tags, request.CacheDuration);
            }

            if (request.CacheDuration.HasValue)
            {
                var command = new CreateCommand()
                {
                    Key = request.CacheKey,
                    Data = response,
                    ExpirationTime = request.CacheDuration.Value / 2,
                    DataType = response.GetType().FullName
                };

                //向redis事务的命令队列插入"发布生成缓存消息"命令
                await _cacheImpl.PublishAsync(CacheKeyPrefix.SyncInMemoryCache, command);
            }

            //执行redis事务命令队列
            await _cacheImpl.CommitTransactionAsync(transaction);

            #endregion

            return response;
        }
    }
}
