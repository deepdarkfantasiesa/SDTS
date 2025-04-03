using MediatR;

namespace Infrastructure.Core
{
    /// <summary>
    /// 查询
    /// </summary>
    /// <typeparam name="TResponse">返回类型</typeparam>
    public interface IQuery<TResponse> : IRequest<TResponse>
    {
        /// <summary>
        /// 偏好缓存等级
        /// </summary>
        public CacheLevelEnum PreferCacheLevel { get; init; }

        /// <summary>
        /// 缓存键
        /// </summary>
        public string? CacheKey { get; }

        /// <summary>
        /// 缓存键上下文
        /// </summary>
        public CacheKeyContext? KeyContext { get; init; }

        /// <summary>
        /// 缓存有效时间
        /// </summary>
        public TimeSpan? CacheDuration { get; init; }

        /// <summary>
        /// 生成缓存键的委托
        /// </summary>
        public Func<CacheKeyContext, string>? Generator { get; init; }
    }
}
