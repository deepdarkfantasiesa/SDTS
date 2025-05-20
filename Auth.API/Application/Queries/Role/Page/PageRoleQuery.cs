using Infrastructure.Core;
using Infrastructure.Core.Query;
using Infrastructure.Core.Query.Page;

namespace Auth.API.Application.Queries.Role.Page
{
    /// <summary>
    /// 分页查询角色入参
    /// </summary>
    public record PageRoleQuery : IQueryBase<PageRequest<QueryCondition>, PageResponse<QueryResult>>, IQueryReplica,IQueryCache<PageResponse<QueryResult>>
    {
        /// <summary>
        /// 
        /// </summary>
        public bool UseReplica { get; init; }

        /// <summary>
        /// 筛选条件
        /// </summary>
        public PageRequest<QueryCondition> Params { get ; init ; }

        public QueryCacheLevel PreferCacheLevel { get; init; } = QueryCacheLevel.None;

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

                _cacheKey = CacheKeyGenerator.Page(KeyContext);

                return _cacheKey;
            }
        }

        public CacheKeyContext? KeyContext { get ; init; }

        public TimeSpan? CacheDuration { get; init; } = TimeSpan.FromMinutes(30);

        public CacheTag[] Tags 
        { 
            get 
            { 
                return new[] 
                { 
                    CacheTag.Query, 
                    CacheTag.Page 
                }; 
            } 
        }
    }
}
