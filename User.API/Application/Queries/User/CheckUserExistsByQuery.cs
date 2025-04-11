using Infrastructure.Core;

namespace User.API.Application.Queries.User
{
    /// <summary>
    /// 校验用户是否存在query
    /// </summary>
    public class CheckUserExistsByQuery : IQuery<bool>
    {
        /// <summary>
        /// 
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 偏好缓存优先级
        /// </summary>
        public CacheLevelEnum PreferCacheLevel { get; init; } = CacheLevelEnum.None;

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

                if (this.PreferCacheLevel != CacheLevelEnum.None && (Generator == null || KeyContext == null))
                    throw new ArgumentNullException("缓存键生成者或缓存键上下文为空");

                _cacheKey = Generator.Invoke(KeyContext);

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
        public CacheTag[]? Tags { get; init; }

        /// <summary>
        /// 生成缓存键的委托
        /// </summary>
        public Func<CacheKeyContext, string>? Generator { get; init; }
    }
}
