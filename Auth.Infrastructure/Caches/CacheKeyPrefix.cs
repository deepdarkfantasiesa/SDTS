namespace Auth.Infrastructure.Caches
{
    /// <summary>
    /// 缓存键的前缀
    /// </summary>
    public class CacheKeyPrefix
    {
        #region 后台任务

        /// <summary>
        /// 后台任务缓存键前缀
        /// </summary>
        private const string BackgroundHost = "BackgroundHost:";

        /// <summary>
        /// pgsql的连接配置
        /// </summary>
        public const string PgSqlsConfig = BackgroundHost + "PgSqlsConfig";

        #endregion

        #region 通道

        /// <summary>
        /// 管道固定前缀
        /// </summary>
        private const string Channel = "Channel:";

        /// <summary>
        /// 同步内存缓存管道
        /// </summary>
        public const string SyncInMemoryCache = Channel + "SyncInMemoryCache";

        #endregion
    }
}
