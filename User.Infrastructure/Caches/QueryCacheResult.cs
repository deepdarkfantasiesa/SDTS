namespace User.Infrastructure.Caches
{
    /// <summary>
    /// 查询缓存的结果
    /// </summary>
    /// <typeparam name="T">缓存数据的类型</typeparam>
    public class QueryCacheResult<T>
    {
        /// <summary>
        /// 是否命中缓存：true命中、false未命中
        /// </summary>
        public bool IsHit { get; set; } = false;

        /// <summary>
        /// 缓存中的数据
        /// </summary>
        public T? Value { get; set; }
    }
}
