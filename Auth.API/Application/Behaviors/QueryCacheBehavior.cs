using Auth.Infrastructure.Caches;
using Auth.Infrastructure.Caches.Models.SyncMemoryCacheCommds;
using Infrastructure.Core;
using Infrastructure.Core.Query;
using MediatR;

namespace Auth.API.Application.Behaviors
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

        private readonly ILogger<QueryCacheBehavior<TRequest, TResponse>> _logger;

        /// <summary>
        /// Query管道行为
        /// </summary>
        /// <param name="cacheImpl">缓存上下文</param>
        public QueryCacheBehavior(ICacheImpl cacheImpl,ILogger<QueryCacheBehavior<TRequest, TResponse>> logger)
        {
            _cacheImpl = cacheImpl;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var response = default(TResponse);

            //判断是否使用缓存
            if (request.PreferCacheLevel == QueryCacheLevel.None)
            {
                _logger.LogInformation("不检索缓存");
                response = await next();
                _logger.LogInformation("检索数据库完成 {@Response}", response);

                return response;
            }

            //判断缓存优先级
            var cacheLevel = request.PreferCacheLevel == QueryCacheLevel.Local ? CacheLevel.Local : CacheLevel.Distributed;

            QueryCacheResult<TResponse> cache = null;
            //查询缓存
            _logger.LogInformation("开始检索缓存{CacheKey} 最高优先级{@PreferCacheLevel}", request.CacheKey, cacheLevel);
            cache = await _cacheImpl.GetHashAsync<TResponse>(request.CacheKey, cacheLevel);
            
            if (cache.IsHit)
            {
                response = cache.Value;
                _logger.LogInformation("命中缓存 {@Response}", response);
                //若命中缓存则直接返回缓存结果
                return response;
            }

            _logger.LogInformation("未命中缓存，开始进行下一个Behavior");

            //运行数据库查询逻辑
            response = await next();

            _logger.LogInformation("开始进行下一个Behavior执行完成");

            #region 插入缓存

            _logger.LogInformation("开启redis事务");

            //开启redis事务
            var transaction = _cacheImpl.BeginTransaction();

            //写入缓存
            if (request.Tags == null || request.Tags.Count() == 0)
            {
                _logger.LogInformation("向redis插入缓存{CacheKey} {@Response} {CacheDuration}", request.CacheKey, response, request.CacheDuration);
                await _cacheImpl.SetHashAsync(request.CacheKey, response, request.CacheDuration);
            }
            else
            {
                _logger.LogInformation("向redis插入缓存{CacheKey} {@Response} {@Tags} {CacheDuration}", request.CacheKey, response, request.Tags, request.CacheDuration);
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

                _logger.LogInformation("向其他服务发布同步缓存通知 {ChannelName} {@Notification}", CacheKeyPrefix.SyncInMemoryCache, command);
            }

            //执行redis事务命令队列
            await _cacheImpl.CommitTransactionAsync(transaction);

            _logger.LogInformation("提交redis事务");
            #endregion

            return response;
        }
    }
}
