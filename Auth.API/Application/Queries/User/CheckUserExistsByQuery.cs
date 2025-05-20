using Infrastructure.Core;
using Infrastructure.Core.Query;

namespace Auth.API.Application.Queries.User
{
    /// <summary>
    /// 校验用户是否存在query
    /// </summary>
    public record CheckUserExistsByQuery : IQueryBase<CheckUserExistParams, bool>, IQueryReplica, IQueryCache<bool>
    {
        /// <summary>
        /// query参数
        /// </summary>
        public CheckUserExistParams Params { get; init; }

        /// <summary>
        /// 偏好缓存优先级
        /// </summary>
        public QueryCacheLevel PreferCacheLevel { get; init; } = QueryCacheLevel.None;

        /// <summary>
        /// 缓存键上下文
        /// </summary>
        public CacheKeyContext? KeyContext { get; init; }

        /// <summary>
        /// 缓存键
        /// </summary>
        private string _cacheKey;

        /// <summary>
        /// 缓存键
        /// </summary>
        public string? CacheKey
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_cacheKey))
                    return _cacheKey;

                if (PreferCacheLevel != QueryCacheLevel.None && (KeyContext == null || KeyContext.Count == 0))
                    throw new ArgumentNullException("缓存键上下文为空");

                _cacheKey = CacheKeyGenerator.Check(KeyContext);

                return _cacheKey;
            }
        }

        /// <summary>
        /// 有效时间
        /// </summary>
        public TimeSpan? CacheDuration { get; init; } = TimeSpan.FromMinutes(30);

        /// <summary>
        /// 标签
        /// </summary>
        public CacheTag[]? Tags
        {
            get
            {
                return new[]
                {
                    CacheTag.Query,
                    CacheTag.CheckIsExist
                };
            }
        }

        /// <summary>
        /// 是否使用从库
        /// </summary>
        public bool UseReplica { get; init; }
    }
}
