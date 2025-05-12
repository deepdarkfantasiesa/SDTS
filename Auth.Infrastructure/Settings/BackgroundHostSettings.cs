namespace Auth.Infrastructure.Settings
{
    /// <summary>
    /// 后台服务轮询配置类
    /// </summary>
    public class BackgroundHostSettings : IEquatable<BackgroundHostSettings>
    {
        /// <summary>
        /// 同步pgsql轮询周期
        /// </summary>
        public int SyncPgSql { get; set; }

        /// <summary>
        /// 比较是否相等
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(BackgroundHostSettings? other)
        {
            if (other == null)
                return false;
            return other.SyncPgSql == SyncPgSql;
        }
    }
}
