using Infrastructure.Core;
using Infrastructure.Core.Query;

namespace User.API.Application.Queries.User
{
    /// <summary>
    /// 通过id查询用户
    /// </summary>
    /// <param name="_dbContext"></param>
    public class GetUserByIdQueryHandler(IQueryDbContext queryContext) : IQueryHandler<GetUserByIdQuery, GetUserByIdParams, UserResponse>
    {
        public IQueryDbContext _queryContext => queryContext;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<UserResponse?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            return await _queryContext
                .QueryFirstOrDefaultAsync<UserResponse>
                (@"SELECT 
                        u.""Id"",
                        u.""Name"" 
                       FROM ""User"" AS u 
                        WHERE u.""Id""=@Id",
                new
                {
                    Id = request.Params.Id
                });
        }
    }
}
