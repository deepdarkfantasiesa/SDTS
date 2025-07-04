using System.Data;

namespace Infrastructure.Core.DatabaseContext
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
        /// <returns></returns>
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null);

        /// <summary>
        /// 单个查询
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="sql">语句</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object param = null);
    }
}
