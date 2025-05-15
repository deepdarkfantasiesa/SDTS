using Infrastructure.Core;
using Infrastructure.Core.Query;
using System.Text;

namespace Auth.API.Application.Queries.Role.Page
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="queryContext">查询上下文</param>
    public class PageRoleQueryHandler(IQueryDbContext queryContext) : IQueryHandler<PageRoleQuery, PageRequest<QueryCondition>, PageResponse<QueryResult>>
    {
        /// <summary>
        /// 查询上下文
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
            var conditions = request.Params.Conditions;
            var result = new PageResponse<QueryResult>
            {
                PageNumber = request.Params.PageNumber,
                PageSize = request.Params.PageSize,
            };

            //分页返回字段
            var columnReplace = result.GetColumnClause();

            //统计
            var countReplace = "COUNT(*)";

            //基本sql
            var baseSql = $@"
             SELECT
               REPLACESQL
             FROM
               role 
             WHERE
               is_deleted = FALSE
            ";

            //主sql
            var pageBuilder = new StringBuilder(baseSql);
            var countBuilder = new StringBuilder(baseSql);

            //参数
            var parameters = new Dictionary<string, object>();

            //拼接过滤条件
            var whereSql = request.Params.GetWhereClause(ref parameters);
            pageBuilder.Append(whereSql);
            countBuilder.Append(whereSql);

            //生成统计sql
            var countString = countBuilder.ToString();
            countString = countString.Replace("REPLACESQL", countReplace);
            var count = await _queryContext.QueryFirstOrDefaultAsync<int>(countString, parameters);

            //排序条件
            var orderSql = request.Params.GetSortClause();
            pageBuilder.Append(orderSql);

            //分页条件
            var pageSql = request.Params.GetPageClause(ref parameters);
            pageBuilder.Append(pageSql);

            //生成分页sql
            var pageString = pageBuilder.ToString();
            pageString = pageString.Replace("REPLACESQL", columnReplace);
            var data = await _queryContext.QueryAsync<QueryResult>(pageString, parameters);

            result.Items = data;
            result.TotalCount = count;
            return result;
        }
    }
}
