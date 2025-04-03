using Infrastructure.Core;

namespace User.API.Application.Queries.User
{
    /// <summary>
    /// 通过id查询用户
    /// </summary>
    public class GetUserByIdQuery : IQuery<UserResponse>
    {
        public int Id { get; set; }
        public CacheLevelEnum PreferCacheLevel { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
        public string? CacheKey { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
        public TimeSpan? CacheDuration { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
        public Func<CacheKeyContext, string>? Generator { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
        public CacheKeyContext? KeyContext { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
    }
}
