using System.Text;

namespace Infrastructure.Core
{
    /// <summary>
    /// 缓存键生成者
    /// </summary>
    public static class CacheKeyGenerator
    {
        /// <summary>
        /// 检查是否存在
        /// </summary>
        /// <param name="keyContexts"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string Check(CacheKeyContext keyContexts)
        {
            StringBuilder cacheKeyBuilder = new StringBuilder("EXIST:");

            foreach (var kvp in keyContexts)
            {
                cacheKeyBuilder.Append($"?{kvp.Key}={kvp.Value}");
            }
            return cacheKeyBuilder.ToString();
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="keyContexts"></param>
        /// <returns></returns>
        public static string Page(CacheKeyContext keyContexts)
        {
            StringBuilder cacheKeyBuilder = new StringBuilder("PAGE:");

            foreach (var kvp in keyContexts)
            {
                cacheKeyBuilder.Append($"?{kvp.Key}={kvp.Value}");
            }
            return cacheKeyBuilder.ToString();
        }
    }
}
