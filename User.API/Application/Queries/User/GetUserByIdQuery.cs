using Domain.Abstraction;

namespace User.API.Application.Queries.User
{
    /// <summary>
    /// 通过id查询用户
    /// </summary>
    public class GetUserByIdQuery : IQuery<UserResponse>
    {
        public int Id { get; set; }
    }
}
