namespace User.Infrastructure.Caches.Models.SyncMemoryCacheCommds
{
	/// <summary>
	/// 基本命令
	/// </summary>
	public record BaseCommand<T>
	{
		/// <summary>
		/// 键
		/// </summary>
		public string CacheKey { get; set; }

		/// <summary>
		/// 命令类型
		/// </summary>
		public CommondType Type { get; set; }

		/// <summary>
		/// 数据
		/// </summary>
		public virtual T? Data { get; set; }

		/// <summary>
		/// 过期时间
		/// </summary>
		public TimeSpan? ExpirationTime { get; set; }

		public string DataType { get; set; }
	}

	/// <summary>
	/// 命令类型
	/// </summary>
	public enum CommondType
	{
		/// <summary>
		/// 创建
		/// </summary>
		Create = 0,

		/// <summary>
		/// 删除
		/// </summary>
		Delete = 1,
	}
}
