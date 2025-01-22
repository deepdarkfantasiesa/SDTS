using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using StackExchange.Redis;
using User.Infrastructure.Caches.Models.SyncMemoryCacheCommds;

namespace User.Infrastructure.Caches.Redis
{
	/// <summary>
	/// 管道消息处理接口
	/// </summary>
	public interface IChannelMessageHandler
	{
		/// <summary>
		/// 同步内存缓存
		/// </summary>
		/// <param name="channel"></param>
		/// <param name="message"></param>
		void SyncInMemoryCache(RedisChannel channel, RedisValue message);
	}

	/// <summary>
	/// 管道消息处理类
	/// </summary>
	public class ChannelMessageHandler: IChannelMessageHandler
	{
		/// <summary>
		/// 内存缓存
		/// </summary>
		private readonly IMemoryCache _memoryCache;

		/// <summary>
		/// 管道消息处理类
		/// </summary>
		/// <param name="memoryCache">内存缓存</param>
		public ChannelMessageHandler(IMemoryCache memoryCache)
		{
			_memoryCache = memoryCache;
		}

		/// <summary>
		/// 同步内存缓存
		/// </summary>
		/// <param name="channel"></param>
		/// <param name="message"></param>
		public void SyncInMemoryCache(RedisChannel channel, RedisValue message)
		{
			// 先反序列化为 BaseCommand<object> 并获取 DataType
			var baseCommand = JsonConvert.DeserializeObject<BaseCommand<object>>(message);
			Type dataType = Type.GetType(baseCommand.DataType);

			// 使用反射创建具体的 BaseCommand<> 类型
			var commandType = typeof(BaseCommand<>).MakeGenericType(dataType);
			var command = JsonConvert.DeserializeObject(message, commandType);
			//var dcommand = (dynamic) command;
			switch(baseCommand.Type)
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
		}
	}
}
