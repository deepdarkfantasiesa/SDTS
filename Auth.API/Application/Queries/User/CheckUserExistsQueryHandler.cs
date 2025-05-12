using Infrastructure.Core;
using Infrastructure.Core.Query;

namespace Auth.API.Application.Queries.User
{
    public class CheckUserExistsQueryHandler(IQueryDbContext queryContext) : IQueryHandler<CheckUserExistsByQuery, CheckUserExistParams, bool>
    {
        public IQueryDbContext _queryContext => queryContext;

        /// <summary>
        /// 校验用户是否存在
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> Handle(CheckUserExistsByQuery request, CancellationToken cancellationToken)
        {
            return await _queryContext.QueryFirstOrDefaultAsync<bool>
                (@"SELECT EXISTS (
                        SELECT 1 FROM ""User"" AS u 
                        WHERE u.""name""=@Name)",
                new
                {
                    Name = request.Params.UserName
                });
        }
    }
}
