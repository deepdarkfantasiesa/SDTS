using Auth.Infrastructure.Caches;
using Auth.Infrastructure.Caches.ImMemory;
using Auth.Infrastructure.Caches.Models.SyncMemoryCacheCommds;
using Infrastructure.Core;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Service.Framework.Models;
using StackExchange.Redis;
using System.Reflection;
using System.Reflection.Metadata;

namespace Auth.Infrastructure.Caches.Redis
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
    public class ChannelMessageHandler : IChannelMessageHandler
    {
        /// <summary>
        /// 内存缓存
        /// </summary>
        private readonly InMemoryCacheContext _memoryCache;

        /// <summary>
        /// 管道消息处理类
        /// </summary>
        /// <param name="memoryCache">内存缓存</param>
        public ChannelMessageHandler(InMemoryCacheContext memoryCache)
        {
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// 同步内存缓存
        /// </summary>
        /// <param name="channel">管道</param>
        /// <param name="message">消息</param>
        public void SyncInMemoryCache(RedisChannel channel, RedisValue message)
        {
            var baseCommand = JsonConvert.DeserializeObject<BaseCommand>(message);

            switch (baseCommand.Type)
            {
                case CommondType.Create:
                    //先序列化创建命令
                    var createCommand = JsonConvert.DeserializeObject<CreateCommand>(message);
                    //拿到数据类型
                    Type dataType = Type.GetType(createCommand.DataType);
                    //反序列化数据为原类型
                    var data = JsonConvert.DeserializeObject(createCommand.Data.ToString().ToLower(), dataType);
                    //写入内存缓存
                    _memoryCache.Set(createCommand.Key, data, new MemoryCacheEntryOptions()
                    {
                        AbsoluteExpiration = DateTimeOffset.Now.Add(createCommand.ExpirationTime)
                    });
                    break;
                case CommondType.Delete:
                    //移除内存缓存中指定键的缓存
                    _memoryCache.Remove(baseCommand.Key);
                    break;
            }
        }

    }
}
