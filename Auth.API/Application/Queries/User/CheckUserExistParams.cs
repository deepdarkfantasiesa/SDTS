using Infrastructure.Core.Query;

namespace Auth.API.Application.Queries.User
{
    /// <summary>
    /// 校验用户是否存在参数类
    /// </summary>
    public class CheckUserExistParams : IQueryParam
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; init; }
    }
}
