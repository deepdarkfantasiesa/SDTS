using Infrastructure.Core;
using Infrastructure.Core.Query;

namespace Auth.API.Application.Queries.Role.Page
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="queryContext"></param>
    public class PageRoleQueryHandler(IQueryDbContext queryContext) : IQueryHandler<PageRoleQuery, PageRequest<QueryCondition>, PageResponse<QueryResult>>
    {
        /// <summary>
        /// 
        /// </summary>
        public IQueryDbContext _queryContext => queryContext;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<PageResponse<QueryResult>> Handle(PageRoleQuery request, CancellationToken cancellationToken)
        {
            var data = await _queryContext.QueryAsync<QueryResult>("", new {});
            return null;
        }
    }
}
