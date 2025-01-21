using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using StackExchange.Redis;
using User.Infrastructure.Caches;
using User.Infrastructure.Caches.Models.SyncMemoryCacheCommds;

namespace User.API.BackgroundHosts
{
	/// <summary>
	/// 同步内存缓存后台任务
	/// </summary>
	public class SyncInMemoryCacheHost : BackgroundService
	{
		/// <summary>
		/// 分布式缓存实现类
		/// </summary>
		private readonly ICacheImpl _cacheImpl;

		/// <summary>
		/// 内存缓存
		/// </summary>
		private readonly IMemoryCache _memoryCache;

		/// <summary>
		/// 同步内存缓存后台任务
		/// </summary>
		/// <param name="cacheImpl"></param>
		/// <param name="memoryCache">内存缓存</param>
		public SyncInMemoryCacheHost(ICacheImpl cacheImpl, IMemoryCache memoryCache)
		{
			_cacheImpl = cacheImpl;
			_memoryCache = memoryCache;
		}

		protected async override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			//订阅同步内存缓存的redis通道
			await _cacheImpl.SubscribeAsync(CacheKeyPrefix.SyncInMemoryCache, (RedisChannel channel, RedisValue message) =>
			{
				// 先反序列化为 BaseCommand<object> 并获取 DataType
				var baseCommand = JsonConvert.DeserializeObject<BaseCommand<object>>(message);
				Type dataType = Type.GetType(baseCommand.DataType);

				// 使用反射创建具体的 BaseCommand<> 类型
				var commandType = typeof(BaseCommand<>).MakeGenericType(dataType);
				var command = JsonConvert.DeserializeObject(message, commandType);
				//var dcommand = (dynamic) command;
				switch (baseCommand.Type)
				{
					case CommondType.Create:
						//通过命令的数据类型从command中反射获取Data
						var data = commandType.GetProperty("Data")?.GetValue(command);
						_memoryCache.Set(baseCommand.CacheKey, data, new MemoryCacheEntryOptions()
						{
							AbsoluteExpiration = DateTimeOffset.Now.Add(baseCommand.ExpirationTime.Value)
						});
						break;
					case CommondType.Delete:
						_memoryCache.Remove(baseCommand.CacheKey);
						break;
				}
			});


		}
	}
}
