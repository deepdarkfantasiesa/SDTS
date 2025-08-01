using Domain.Abstraction.Mediator;
using Infrastructure.Core.Cache;
//using MediatR;

namespace Infrastructure.Core.Query
{
    /// <summary>
    /// query缓存
    /// </summary>
    /// <typeparam name="TResponse">返回类型</typeparam>
    public interface IQueryCache<TResponse> : IRequest<TResponse>
    {
        /// <summary>
        /// 偏好缓存等级
        /// </summary>
        public QueryCacheLevel PreferCacheLevel { get; init; }

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
        /// 标签
        /// </summary>
        public CacheTag[] Tags { get; }
    }
}
