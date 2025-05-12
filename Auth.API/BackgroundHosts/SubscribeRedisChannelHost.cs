using Auth.Infrastructure.Caches;
using Auth.Infrastructure.Caches.Redis;

namespace Auth.API.BackgroundHosts
{
    /// <summary>
    /// 订阅reids管道后台任务
    /// </summary>
    public class SubscribeRedisChannelHost : BackgroundService
    {
        /// <summary>
        /// 消息处理者
        /// </summary>
        private readonly IChannelMessageHandler _handler;

        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// 订阅reids管道后台任务
        /// </summary>
        /// <param name="cacheImpl">分布式缓存实现类</param>
        /// <param name="handler">消息处理者</param>
        public SubscribeRedisChannelHost(IChannelMessageHandler handler, IServiceProvider serviceProvider)
        {
            _handler = handler;
            _serviceProvider = serviceProvider;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var _cacheImpl = scope.ServiceProvider.GetService<ICacheImpl>() ?? throw new ArgumentNullException("未获取到缓存操作类");

                //订阅同步内存缓存的redis通道
                await _cacheImpl.SubscribeAsync(CacheKeyPrefix.SyncInMemoryCache, _handler.SyncInMemoryCache);
            }

        }
    }
}
