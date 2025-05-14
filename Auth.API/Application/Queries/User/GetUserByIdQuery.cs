using Infrastructure.Core.Query;

namespace Auth.API.Application.Queries.User
{
    /// <summary>
    /// 通过id查询用户
    /// </summary>
    public class GetUserByIdQuery : IQueryBase<GetUserByIdParams, UserResponse>
    {
        public GetUserByIdParams Params { get; init; }
    }
}
