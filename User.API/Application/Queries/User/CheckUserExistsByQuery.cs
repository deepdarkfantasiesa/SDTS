using Domain.Abstraction;

namespace User.API.Application.Queries.User
{
    /// <summary>
    /// 校验用户是否存在query
    /// </summary>
    public class CheckUserExistsByQuery : IQuery<bool>
    {
        public string UserName { get; set; }
    }
}
