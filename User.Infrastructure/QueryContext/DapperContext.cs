using Dapper;
using System.Data;

namespace User.Infrastructure.QueryContext
{
    /// <summary>
    /// dapper数据库上下文
    /// </summary>
    public class DapperContext : IQueryDbContext
    {
        /// <summary>
        /// 连接实例
        /// </summary>
        private readonly IDbConnection _connection;

        /// <summary>
        /// dapper数据库上下文
        /// </summary>
        /// <param name="connection">连接实例</param>
        public DapperContext(IDbConnection connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionString 
        {
            get => _connection.ConnectionString;
            set
            {
                //服务发现


                _connection.ConnectionString = value;
            } 
        }

        /// <summary>
        /// 数据库连接超时时间
        /// </summary>
        public int ConnectionTimeout => throw new NotImplementedException();

        /// <summary>
        /// 
        /// </summary>
        public string Database => _connection.Database;

        /// <summary>
        /// 连接状态
        /// </summary>
        public ConnectionState State => _connection.State;

        #region 开启事务

        public IDbTransaction BeginTransaction()
        {
            throw new NotImplementedException("该上下文只能用于查询");
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            throw new NotImplementedException("该上下文只能用于查询");
        }

        #endregion

        #region 其他方法

        public void ChangeDatabase(string databaseName)
        {
            throw new NotImplementedException();
        }

        public IDbCommand CreateCommand()
        {
            throw new NotImplementedException("不可使用该函数");
        }

        #endregion

        #region 资源释放

        private bool _disposed = false;

        public void Dispose()
        {
            if (!_disposed)
            {
                _connection.Dispose();
                _disposed = true;
            }

            GC.SuppressFinalize(this);
        }

        #endregion

        #region 开启/关闭连接

        /// <summary>
        /// 开启连接
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Open()
        {
            throw new NotImplementedException("无需手动开启连接");
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Close()
        {
            throw new NotImplementedException("不能手动关闭连接");
        }

        #endregion

        #region 查询方法

        /// <summary>
        /// 列表查询
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="sql">语句</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> ToListAsync<T>(string sql, object param = null)
        {
            ValidateSql(sql);
            return await _connection.QueryAsync<T>(sql, param);
        }

        /// <summary>
        /// 单个查询
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="sql">语句</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public async Task<T?> FirstOrDefaultAsync<T>(string sql, object param = null)
        {
            ValidateSql(sql);
            return await _connection.QueryFirstOrDefaultAsync<T>(sql, param);
        }

        #endregion

        /// <summary>
        /// 校验 SQL 语句是否以 SELECT 开头
        /// </summary>
        /// <param name="sql"></param>
        /// <exception cref="InvalidOperationException"></exception>
        private void ValidateSql(string sql)
        {
            if (!sql.TrimStart().StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("该上下文只能用于查询");
            }
        }
    }
}
