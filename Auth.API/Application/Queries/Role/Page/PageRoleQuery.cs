using Infrastructure.Core.Query;

namespace Auth.API.Application.Queries.Role.Page
{
    /// <summary>
    /// 分页查询角色入参
    /// </summary>
    public record PageRoleQuery : IQueryBase<PageRequest<QueryCondition>, PageResponse<QueryResult>>, IQueryReplica<PageResponse<QueryResult>>
    {
        /// <summary>
        /// 
        /// </summary>
        public bool UseReplica { get; init; }

        /// <summary>
        /// 筛选条件
        /// </summary>
        public PageRequest<QueryCondition> Params { get ; init ; }
    }
}
