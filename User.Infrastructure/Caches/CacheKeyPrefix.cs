using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User.Infrastructure.Caches
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
	}
}
