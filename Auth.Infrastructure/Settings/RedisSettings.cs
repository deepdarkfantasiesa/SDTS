namespace Auth.Infrastructure.Settings
{
    /// <summary>
    /// redis配置类
    /// </summary>
    public class RedisSettings : IEquatable<RedisSettings>
    {
        /// <summary>
        /// 默认过期时间（秒）
        /// </summary>
        public int DefaultExpirationTime { get; set; }

        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 默认数据库
        /// </summary>
        public int DefaultDbNumber { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 实例数量
        /// </summary>
        public int InstanceCount { get; set; }

        /// <summary>
        /// 节点配置
        /// </summary>
        public List<RedisConfigEndPoint> EndPoints { get; set; }

        /// <summary>
        /// 比较是否相等
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(RedisSettings? other)
        {
            if (other == null)
                return false;

            return ConnectionString == other.ConnectionString
                && DefaultDbNumber == other.DefaultDbNumber
                && Password == other.Password
                && InstanceCount == other.InstanceCount
                && EndPoints.SequenceEqual(other.EndPoints);
        }
    }

    /// <summary>
    /// 节点配置类
    /// </summary>
    public class RedisConfigEndPoint : IEquatable<RedisConfigEndPoint>
    {
        /// <summary>
        /// 地址
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 比较是否相等
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(RedisConfigEndPoint? other)
        {
            if (other == null)
                return false;
            return Host == other.Host && Port == other.Port;
        }
    }
}
