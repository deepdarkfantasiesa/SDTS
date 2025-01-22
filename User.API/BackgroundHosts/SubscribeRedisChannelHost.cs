using User.Infrastructure.Caches;
using User.Infrastructure.Caches.Redis;

namespace User.API.BackgroundHosts
{
	/// <summary>
	/// 订阅reids管道后台任务
	/// </summary>
	public class SubscribeRedisChannelHost: BackgroundService
	{
		/// <summary>
		/// 分布式缓存实现类
		/// </summary>
		private readonly ICacheImpl _cacheImpl;

		/// <summary>
		/// 消息处理者
		/// </summary>
		private readonly IChannelMessageHandler _handler;

		/// <summary>
		/// 订阅reids管道后台任务
		/// </summary>
		/// <param name="cacheImpl">分布式缓存实现类</param>
		/// <param name="handler">消息处理者</param>
		public SubscribeRedisChannelHost(ICacheImpl cacheImpl, IChannelMessageHandler handler)
		{
			_cacheImpl = cacheImpl;
			_handler = handler;
		}

		protected async override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			//订阅同步内存缓存的redis通道
			await _cacheImpl.SubscribeAsync(CacheKeyPrefix.SyncInMemoryCache, _handler.SyncInMemoryCache);
		}
	}
}
