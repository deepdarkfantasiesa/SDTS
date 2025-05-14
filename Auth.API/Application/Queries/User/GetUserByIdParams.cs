using Infrastructure.Core.Query;

namespace Auth.API.Application.Queries.User
{
    /// <summary>
    /// 通过id获取用户参数类
    /// </summary>
    public class GetUserByIdParams : IQueryParam
    {
        public string Id { get; set; }
    }
}
