using Infrastructure.Core;
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
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        public QueryCacheResult<T> Get<T>(string key,CacheTag[] tags)
        {
            var isMatch = AreAllTagsMatching(key, tags);
            if (isMatch == false)
                return new QueryCacheResult<T> { IsHit = false, Value = default(T) };
            
            if(_memoryCache.TryGetValue<T>(key, out var result))
            {
                return new QueryCacheResult<T> { IsHit = true, Value = result };
            }
            else
            {
                return new QueryCacheResult<T> { IsHit = false, Value = default(T) };
            }
        }

        /// <summary>
        /// 检查传入的 tags 中的所有标签是否都包含指定的 key。
        /// </summary>
        /// <param name="key">要检查的缓存键。</param>
        /// <param name="tags">要检索的标签数组。</param>
        /// <returns>如果所有标签的 HashSet 都包含该 key，则返回 true；否则返回 false。</returns>
        private bool AreAllTagsMatching(string key, CacheTag[] tags)
        {
            foreach (var tag in tags)
            {
                // 检查标签是否存在于 Tags 字典中
                if (!Tags.TryGetValue(tag, out var hashSet) || !hashSet.Contains(key))
                {
                    // 如果标签不存在，或者 HashSet 不包含 key，则返回 false
                    return false;
                }
            }

            // 如果所有标签都匹配，则返回 true
            return true;
        }
    }
}
