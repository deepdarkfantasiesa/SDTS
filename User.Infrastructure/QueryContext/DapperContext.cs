using Dapper;
using Infrastructure.Core;
using Npgsql;
using Service.Framework.Models;
using Service.Framework.ServiceRegistry;
using System.Data;
using User.Infrastructure.Caches;

namespace User.Infrastructure.QueryContext
{
    /// <summary>
    /// dapper数据库上下文
    /// </summary>
    public class DapperContext : IQueryDbContext
    {
        /// <summary>
        /// 缓存
        /// </summary>
        private readonly ICacheImpl _cache;

        /// <summary>
        /// 服务发现中心
        /// </summary>
        private readonly IRegistryService _serviceCenter;

        /// <summary>
        /// 连接实例
        /// </summary>
        private readonly IDbConnection _connection;

        /// <summary>
        /// dapper数据库上下文
        /// </summary>
        /// <param name="connstr">Db连接字符串</param>
        /// <param name="cache">缓存</param>
        /// <param name="serviceCenter">服务发现中心</param>
        public DapperContext(string connstr, ICacheImpl cache, IRegistryService serviceCenter)
        {
            _connection = new NpgsqlConnection(connstr);
            _cache = cache;
            _serviceCenter = serviceCenter;
        }

        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionString 
        {
            get => _connection.ConnectionString;
            set => _connection.ConnectionString = value;
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

        public System.Data.IDbTransaction BeginTransaction()
        {
            throw new NotImplementedException("该上下文只能用于查询");
        }

        public System.Data.IDbTransaction BeginTransaction(IsolationLevel il)
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
        /// <param name="useReplica">是否使用从库</param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, bool useReplica = false)
        {
            ValidateSql(sql);
            if (useReplica)
            {
                await TrySetConnStrToReplica();
            }
            return await _connection.QueryAsync<T>(sql, param);
        }

        /// <summary>
        /// 单个查询
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="sql">语句</param>
        /// <param name="param">参数</param>
        /// <param name="useReplica">是否使用从库</param>
        /// <returns></returns>
        public async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object param = null, bool useReplica = false)
        {
            ValidateSql(sql);
            if (useReplica)
            {
                await TrySetConnStrToReplica();
            }
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

        /// <summary>
		/// 尝试设置连接字符串为从库
		/// </summary>
		private async Task TrySetConnStrToReplica()
        {
            //从缓存中获取数据库实例的信息
            var cacheResult = await _cache.GetHashAsync<IEnumerable<RelationalDatabaseModel>>(CacheKeyPrefix.PgSqlsConfig, CacheLevel.Local);

            var rdbs = cacheResult.IsHit == true ? cacheResult.Value : null;

            if (rdbs == null || rdbs.Count() == 0)
            {
                rdbs = await _serviceCenter.DiscoverRDB("pgsql");
            }

            var radom = new Random();

            var replicaRdbs = rdbs.Where(p => p.Tag.Contains("replica")).ToList();
            if (replicaRdbs != null && replicaRdbs.Count > 0)
            {
                var replicaRdb = replicaRdbs[radom.Next(replicaRdbs.Count)];
                _connection.ConnectionString = $"Host={replicaRdb.Address}:{replicaRdb.Port};Database=postgres;Username=postgres;Password=postgres";
            }
            else
            {
                Console.WriteLine("设置从库连接字符串失败");
            }
        }
    }
}
