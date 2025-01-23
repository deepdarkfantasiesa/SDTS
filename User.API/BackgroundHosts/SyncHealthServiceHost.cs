using Microsoft.Extensions.Options;
using Service.Framework.Models;
using Service.Framework.ServiceRegistry;
using User.Infrastructure.Caches;
using User.Infrastructure.Caches.Models.SyncMemoryCacheCommds;
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
		/// <param name="bgHostSettings">后台任务轮询配置模块</param>
		/// <param name="serviceProvider"></param>
		public SyncHealthServiceHost(IOptionsMonitor<BackgroundHostSettings> bgHostSettings, IServiceProvider serviceProvider)
		{
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
			try
			{
				using(var scope = _serviceProvider.CreateAsyncScope())
				{
					var registryService = scope.ServiceProvider.GetService<IRegistryService>() ?? throw new ArgumentNullException("服务发现类为空");

					var _cacheImpl = scope.ServiceProvider.GetService<ICacheImpl>() ?? throw new ArgumentNullException("缓存操作类为空");

					//从服务发现中心获取指定名称的关系型数据库配置
					var rdbConfigs = await registryService.DiscoverRDB("pgsql");

					var transaction = _cacheImpl.BeginTransaction();

					//写入缓存
					await _cacheImpl.SetStringAsync(CacheKeyPrefix.PgSqlsConfig, rdbConfigs, TimeSpan.FromSeconds(20));

					var command = new CreateCommand<object>() { CacheKey = CacheKeyPrefix.PgSqlsConfig, Data = rdbConfigs, ExpirationTime = TimeSpan.FromSeconds(10), DataType = rdbConfigs.GetType().FullName };

					await _cacheImpl.PublishAsync(CacheKeyPrefix.SyncInMemoryCache, command);

					await _cacheImpl.CommitTransactionAsync(transaction);
					//await _cacheImpl.Test(CacheKeyPrefix.PgSqlsConfig, rdbConfigs, TimeSpan.FromSeconds(20));
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"{ex.Message}");
			}
		}
	}
}
