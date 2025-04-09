using Infrastructure.Core;
using Infrastructure.Core.Extension;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;

namespace User.Infrastructure.Caches.ImMemory
{
    /// <summary>
    /// 内存缓存上下文
    /// </summary>
    public class InMemoryCacheContext
    {
        /// <summary>
		/// 内存缓存
		/// </summary>
		private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// 标签，key为标签，value为键值
        /// </summary>
        public ConcurrentDictionary<CacheTag, HashSet<string>> Tags { get; private set; }

        /// <summary>
        /// 内存缓存上下文
        /// </summary>
        /// <param name="memoryCache">内存缓存</param>
        public InMemoryCacheContext(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            Tags = new ConcurrentDictionary<CacheTag, HashSet<string>>();
        }

        /// <summary>
        /// 插入缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        public object Set(string key, object value, CacheTag[] tags, MemoryCacheEntryOptions options)
        {
            foreach (var tag in tags)
            {
                Tags.AddOrUpdate(tag,
                    _ => new HashSet<string> { key },
                    (_, existingKeys) =>
                    {
                        lock (existingKeys) // 确保线程安全
                        {
                            existingKeys.Add(key);
                        }
                        return existingKeys;
                    });
            }

            return _memoryCache.Set(key, value, options);
        }

        /// <summary>
        /// 插入缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public object Set(string key, object value, MemoryCacheEntryOptions options)
        {
            return _memoryCache.Set(key, value, options);
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public QueryCacheResult<T> Get<T>(string key)
        {
            if (_memoryCache.TryGetValue<T>(key, out var result))
            {
                return new QueryCacheResult<T> { IsHit = true, Value = result };
            }
            else
            {
                return new QueryCacheResult<T> { IsHit = false, Value = default(T) };
            }
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            _memoryCache.Remove(key);
            RemoveExpireTagValue(EnumExtensions.GetAllValuesAsArray<CacheTag>(), key);
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="tags">标签</param>
        public void Remove(CacheTag[] tags)
        {
            foreach(var tag in tags)
            {
                Tags.TryRemove(tag, out _);
            }
        }

        /// <summary>
        /// 移除标签中过期的值
        /// </summary>
        /// <param name="tag">标签</param>
        /// <param name="key">键</param>
        /// <returns></returns>
        public void RemoveExpireTagValue(CacheTag tag, string key)
        {
            if (Tags.TryGetValue(tag, out var keys))
            {
                keys.Remove(key);
                
                // 如果键集合为空，移除整个标签
                if (keys.Count == 0)
                {
                    Tags.TryRemove(tag, out _);
                }
            }
        }

        /// <summary>
        /// 移除标签中过期的值
        /// </summary>
        /// <param name="tags">标签</param>
        /// <param name="key">键</param>
        /// <returns></returns>
        public void RemoveExpireTagValue(CacheTag[] tags, string key)
        {
            foreach(var tag in tags)
            {
                RemoveExpireTagValue(tag, key);
            }
            
        }

    }
}
