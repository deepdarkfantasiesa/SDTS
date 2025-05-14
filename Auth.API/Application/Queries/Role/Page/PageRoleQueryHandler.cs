using Dapper;
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

            //分页返回字段
            var columnReplace = "id as Id,name as Name,description as Description,create_at as CreateAt,update_at as UpdateAt";
            
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

            //sql变量值
            var sqlParams = new DynamicParameters();

            //拼接条件
            if (conditions != null)
            {
                if (!string.IsNullOrWhiteSpace(conditions.Name))
                {
                    var filter = "and (name like '%'|| @Name ||'%') ";
                    pageBuilder.Append(filter);
                    countBuilder.Append(filter);
                    sqlParams.Add("Name", conditions.Name);
                }
                if (!string.IsNullOrWhiteSpace(conditions.Description))
                {
                    var filter = "and (description like '%'|| @Description ||'%') ";
                    pageBuilder.Append(filter);
                    countBuilder.Append(filter);
                    sqlParams.Add("Description", conditions.Description);
                }
                if (conditions.CreateAtStart.HasValue)
                {
                    var filter = "and (create_at >= @CreateAtStart) ";
                    pageBuilder.Append(filter);
                    countBuilder.Append(filter);
                    sqlParams.Add("CreateAtStart", conditions.CreateAtStart);
                }
                if (conditions.CreateAtEnd.HasValue)
                {
                    var filter = "and (create_at <= @CreateAtEnd) ";
                    pageBuilder.Append(filter);
                    countBuilder.Append(filter);
                    sqlParams.Add("CreateAtEnd", conditions.CreateAtEnd);
                }
                if (conditions.UpdateAtStart.HasValue)
                {
                    var filter = "and (update_at >= @UpdateAtStart) ";
                    pageBuilder.Append(filter);
                    countBuilder.Append(filter);
                    sqlParams.Add("UpdateAtStart", conditions.UpdateAtStart);
                }
                if (conditions.UpdateAtEnd.HasValue)
                {
                    var filter = "and (update_at <= @UpdateAtEnd) ";
                    pageBuilder.Append(filter);
                    countBuilder.Append(filter);
                    sqlParams.Add("UpdateAtEnd", conditions.UpdateAtEnd);
                }
            }

            //排序条件
            var orderSql = request.Params.GetSortClause();
            pageBuilder.Append(orderSql);

            //分页条件
            pageBuilder.Append("LIMIT @PageSize OFFSET (@PageNumber - 1) * @PageSize ");
            sqlParams.Add("PageNumber", request.Params.PageNumber);
            sqlParams.Add("PageSize", request.Params.PageSize);

            //生成分页sql
            var pageString = pageBuilder.ToString();
            pageString = pageString.Replace("REPLACESQL", columnReplace);
            var data = await _queryContext.QueryAsync<QueryResult>(pageString, sqlParams);

            //生成统计sql
            var countString = countBuilder.ToString();
            countString= countString.Replace("REPLACESQL", countReplace);
            var count = await _queryContext.QueryFirstOrDefaultAsync<int>(countString, sqlParams);

            return new PageResponse<QueryResult>
            {
                Items = data,
                PageNumber = request.Params.PageNumber,
                PageSize = request.Params.PageSize,
                TotalCount = count
            };
        }
    }
}
