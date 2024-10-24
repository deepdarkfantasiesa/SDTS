namespace User.Infrastructure.Settings
{
    /// <summary>
    /// redis配置类
    /// </summary>
    public class RedisSettings
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 默认数据库
        /// </summary>
        public int DefaultDbNumber { get; set; }
    }
}
