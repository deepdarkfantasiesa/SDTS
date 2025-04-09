using Infrastructure.Core;

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

		///// <summary>
		///// 数据
		///// </summary>
		//public virtual T? Data { get; set; }

		/// <summary>
		/// 过期时间
		/// </summary>
		public TimeSpan? ExpirationTime { get; set; }

		/// <summary>
		/// 数据类型
		/// </summary>
		public string DataType { get; set; }

		///// <summary>
		///// 标签
		///// </summary>
		//public CacheTag[]? Tags { get; set; }
	}
}
