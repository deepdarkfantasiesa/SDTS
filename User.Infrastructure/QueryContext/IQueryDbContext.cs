using System.Data;

namespace User.Infrastructure.QueryContext
{
    /// <summary>
    /// 查询上下文接口
    /// </summary>
    public interface IQueryDbContext : IDbConnection
    {
        /// <summary>
        /// 列表查询
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="sql">语句</param>
        /// <param name="param">参数</param>
        /// <param name="useReplica">是否使用从库</param>
        /// <returns></returns>
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, bool useReplica = false);

        /// <summary>
        /// 单个查询
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="sql">语句</param>
        /// <param name="param">参数</param>
        /// <param name="useReplica">是否使用从库</param>
        /// <returns></returns>
        Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object param = null, bool useReplica = false);
    }
}
