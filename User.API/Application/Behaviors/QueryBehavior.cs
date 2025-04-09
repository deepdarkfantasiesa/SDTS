using Infrastructure.Core;
using MediatR;
using User.Infrastructure.Caches;
using User.Infrastructure.Caches.Models.SyncMemoryCacheCommds;

namespace User.API.Application.Behaviors
{
    /// <summary>
    /// Query管道行为
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class QueryBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IQuery<TResponse>
    {
        /// <summary>
        /// 缓存上下文
        /// </summary>
        private readonly ICacheImpl _cacheImpl;

        /// <summary>
        /// Query管道行为
        /// </summary>
        /// <param name="cacheImpl">缓存上下文</param>
        public QueryBehavior(ICacheImpl cacheImpl)
        {
            _cacheImpl = cacheImpl;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            //判断是否使用缓存
            if (request.PreferCacheLevel == CacheLevelEnum.None)
                return await next();

            //判断缓存优先级
            var preferInMemory = request.PreferCacheLevel == CacheLevelEnum.Memory ? true : false;

            QueryCacheResult<TResponse> cache = null;
            //查询缓存
            cache = await _cacheImpl.GetStringAsync<TResponse>(request.CacheKey, preferLocal: preferInMemory);

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
                await _cacheImpl.SetStringAsync(request.CacheKey, response, request.CacheDuration);
            }
            else
            {
                await _cacheImpl.SetStringAsync(request.CacheKey, response, tags: request.Tags, request.CacheDuration);
            }

            var command = new CreateCommand<TResponse>()
            {
                CacheKey = request.CacheKey,
                Data = response,
                ExpirationTime = request.CacheDuration / 2,
                DataType = response.GetType().FullName,
                Tags = request.Tags
            };

            //向redis事务的命令队列插入"发布生成缓存消息"命令
            await _cacheImpl.PublishAsync(CacheKeyPrefix.SyncInMemoryCache, command);

            //执行redis事务命令队列
            await _cacheImpl.CommitTransactionAsync(transaction);

            #endregion

            return response;
        }
    }
}
