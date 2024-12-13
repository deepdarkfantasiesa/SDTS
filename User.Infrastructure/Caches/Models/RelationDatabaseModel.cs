namespace User.Infrastructure.Caches.Models
{
	/// <summary>
	/// 关系数据库的模型
	/// </summary>
	public class RelationDatabaseModel
	{
		/// <summary>
		/// 地址
		/// </summary>
		public string Address { get; set; }

		/// <summary>
		/// 端口
		/// </summary>
		public int Port { get; set; }

		/// <summary>
		/// 标签
		/// </summary>
		public string[] Tag { get; set; }
	}
}
