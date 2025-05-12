using Auth.Infrastructure.Caches;
using Microsoft.Extensions.Caching.Memory;

namespace Auth.Infrastructure.Caches.ImMemory
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
        /// 内存缓存上下文
        /// </summary>
        /// <param name="memoryCache">内存缓存</param>
        public InMemoryCacheContext(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
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
                return new QueryCacheResult<T> { IsHit = false, Value = default };
            }
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            _memoryCache.Remove(key);
        }
    }
}
