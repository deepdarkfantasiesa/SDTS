namespace User.Infrastructure.Settings
{
	/// <summary>
	/// 后台服务轮询配置类
	/// </summary>
	public class BackgroundHostSettings
	{
		/// <summary>
		/// 同步pgsql轮询周期
		/// </summary>
		public int SyncPgSql {  get; set; }
	}
}
