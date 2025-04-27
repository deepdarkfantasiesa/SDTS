using Infrastructure.Core.Query;
using User.Infrastructure.QueryContext;

namespace User.API.Application.Queries.User
{
    public class CheckUserExistsQueryHandler(IQueryDbContext dbContext) : IQueryHandler<CheckUserExistsByQuery, bool>
    {
        /// <summary>
        /// 校验用户是否存在
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> Handle(CheckUserExistsByQuery request, CancellationToken cancellationToken)
        {
            return await dbContext.QueryFirstOrDefaultAsync<bool>
                (@"SELECT EXISTS (
                        SELECT 1 FROM ""User"" AS u 
                        WHERE u.""Name""=@Name)",
                new
                {
                    Name = request.UserName
                });
        }
    }
}
