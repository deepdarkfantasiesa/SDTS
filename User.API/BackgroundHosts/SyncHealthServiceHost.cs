using Microsoft.Extensions.Options;
using Service.Framework.ServiceRegistry;
using User.Infrastructure.Caches;
using User.Infrastructure.Caches.Models;
using User.Infrastructure.Settings;

namespace User.API.BackgroundHosts
{
	/// <summary>
	/// 从服务发现中心同步健康服务后台任务
	/// </summary>
	public class SyncHealthServiceHost : BackgroundService
	{
		/// <summary>
		/// 后台任务轮询配置类
		/// </summary>
		private BackgroundHostSettings _bgHostSettings { get; set; }

		/// <summary>
		/// 缓存实现类
		/// </summary>
		private readonly ICacheImpl _cacheImpl;

		/// <summary>
		/// 同步pgsql定时器
		/// </summary>
		private Timer _syncPgSqltimer;

		/// <summary>
		/// 
		/// </summary>
		private IServiceProvider _serviceProvider;

		/// <summary>
		/// 从服务发现中心同步健康服务后台任务
		/// </summary>
		/// <param name="cacheImpl">缓存实现类</param>
		/// <param name="bgHostSettings">后台任务轮询配置模块</param>
		/// <param name="serviceProvider"></param>
		public SyncHealthServiceHost(ICacheImpl cacheImpl, IOptionsMonitor<BackgroundHostSettings> bgHostSettings, IServiceProvider serviceProvider)
		{
			_cacheImpl = cacheImpl;
			bgHostSettings.OnChange(OnConfigurationChange);
			_bgHostSettings = bgHostSettings.CurrentValue;
			_serviceProvider = serviceProvider;
		}

		/// <summary>
		/// 配置变更监听方法
		/// </summary>
		/// <param name="bgHostSettings"></param>
		private async void OnConfigurationChange(BackgroundHostSettings bgHostSettings)
		{
			if (_bgHostSettings.Equals(bgHostSettings)) return;
			Console.WriteLine("BackgroundHostSettings Changed");
			_bgHostSettings = bgHostSettings;
			_syncPgSqltimer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(_bgHostSettings.SyncPgSql));
		}

		/// <summary>
		/// 执行后台任务
		/// </summary>
		/// <param name="stoppingToken"></param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		protected override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			_syncPgSqltimer = new Timer(SyncPgSqlTask, null, TimeSpan.Zero, TimeSpan.FromSeconds(_bgHostSettings.SyncPgSql));

			return Task.CompletedTask;
		}

		/// <summary>
		/// 同步pgsql的任务
		/// </summary>
		/// <param name="state"></param>
		private async void SyncPgSqlTask(object state)
		{
			using (var scope = _serviceProvider.CreateAsyncScope())
			{
				var registryService = scope.ServiceProvider.GetService<IRegistryService>();
				var discoverServices = await registryService.Discover("pgsql");
				var rdbConfigs = new List<RelationDatabaseModel>();
				foreach (var discoverService in discoverServices.Response)
				{
					rdbConfigs.Add(new RelationDatabaseModel
					{
						Address = discoverService.Service.Address,
						Port = discoverService.Service.Port,
						Tag = discoverService.Service.Tags
					});
				}
				await _cacheImpl.SetStringAsync(CacheKeyPrefix.PgSqlsConfig, rdbConfigs, TimeSpan.FromSeconds(20));
			}
		}
	}
}
