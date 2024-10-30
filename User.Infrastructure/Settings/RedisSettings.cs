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

        /// <summary>
        /// 密码
        /// </summary>
        public string Password {  get; set; }

		/// <summary>
		/// 实例数量，只读实例的数量=InstanceCount*从库的数量
		/// </summary>
		public int InstanceCount {  get; set; }

	}
}
