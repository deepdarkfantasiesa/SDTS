using Infrastructure.Core;

namespace User.Infrastructure.Caches.Models.SyncMemoryCacheCommds
{
    /// <summary>
    /// 创建命令
    /// </summary>
    public record CreateCommand<T> : BaseCommand<T>
	{
		public CommondType Type { get; } = CommondType.Create;

        /// <summary>
		/// 标签
		/// </summary>
		public CacheTag[]? Tags { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
		/// 数据类型
		/// </summary>
		public string DataType { get; set; }
    }
}
