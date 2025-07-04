using Infrastructure.Core.DatabaseContext;
using Infrastructure.Core.Query;
using Infrastructure.Core.Query.Page;
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
            var result = new PageResponse<QueryResult>();

            //基本sql
            //COLUMNClAUSE：待替换的列sql
            //CONDITIONClAUSE：待替换的过滤sql
            //ORDERCLAUSE：待替换的排序sql
            //PAGECLAUSE：待替换的分页sql
            var baseSql = "SELECT COLUMNClAUSE FROM role WHERE is_deleted = FALSE CONDITIONClAUSE ORDERCLAUSE PAGECLAUSE";
            var countSql = "SELECT COUNT(1) FROM role WHERE is_deleted = FALSE CONDITIONClAUSE";

            //主sql
            var pageBuilder = new StringBuilder(baseSql);
            var countBuilder = new StringBuilder(countSql);

            //参数集合
            var parameters = new Dictionary<string, object>();

            //过滤条件
            var whereSql = request.Params.GetConditionClause(ref parameters);
            pageBuilder.ReplaceCondition(whereSql);
            countBuilder.ReplaceCondition(whereSql);

            //统计查询
            var count = await _queryContext.QueryFirstOrDefaultAsync<int>(countBuilder.ToString(), parameters);

            //排序条件
            var orderSql = request.Params.GetOrderClause();
            pageBuilder.ReplaceOrder(orderSql);

            //分页条件
            var pageSql = request.Params.GetPageClause(ref parameters);
            pageBuilder.ReplacePage(pageSql);

            //分页返回字段
            var columnReplace = result.GetColumnClause();
            pageBuilder.ReplaceColumn(columnReplace);

            //分页查询
            var data = await _queryContext.QueryAsync<QueryResult>(pageBuilder.ToString(), parameters);

            result.Items = data;
            result.TotalCount = count;
            result.PageNumber = request.Params.PageNumber;
            result.PageSize = request.Params.PageSize;
            return result;
        }
    }
}
