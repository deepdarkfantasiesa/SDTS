using Auth.Infrastructure.Caches;
using Infrastructure.Core.Cache;
using Infrastructure.Core.Extension;
using RedLockNet.SERedis;

namespace Auth.API.BackgroundHosts
{
    /// <summary>
    /// 清理过期tag值后台任务
    /// </summary>
    public class RedisTagCleanupHost : BackgroundService
    {
        /// <summary>
        /// 红锁
        /// </summary>
        private readonly RedLockFactory _redLockFactory;

        /// <summary>
        /// 定时器
        /// </summary>
        private Timer _timer;

        /// <summary>
        /// 
        /// </summary>
        private readonly IServiceProvider _serviceProvider;


        /// <summary>
        /// 清理过期tag值后台任务
        /// </summary>
        /// <param name="redLockFactory">红锁</param>
        /// <param name="serviceProvider"></param>
        public RedisTagCleanupHost(RedLockFactory redLockFactory, IServiceProvider serviceProvider)
        {
            _redLockFactory = redLockFactory;
            _serviceProvider = serviceProvider;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(TagCleanUpTask, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));

            return Task.CompletedTask;
        }

        /// <summary>
		/// 清理过期tag值任务
		/// </summary>
		/// <param name="state"></param>
		private async void TagCleanUpTask(object state)
        {
            using (var redLock = await _redLockFactory.CreateLockAsync("TagCleanupTask", TimeSpan.FromSeconds(30)))
            {
                if (!redLock.IsAcquired)
                {
                    return;
                }

                using (var scope = _serviceProvider.CreateScope())
                {
                    var _cacheImpl = scope.ServiceProvider.GetService<ICacheImpl>() ?? throw new ArgumentNullException("未获取到缓存操作类");

                    await _cacheImpl.RemoveExpireTagValue(EnumExtensions.GetAllValuesAsArray<CacheTag>());
                }
            }
        }
    }
}
