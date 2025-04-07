namespace User.Infrastructure.Caches.Models.SyncMemoryCacheCommds
{
    /// <summary>
    /// 删除命令
    /// </summary>
    public record DeleteCommand<T> : BaseCommand<T>
	{
		/// <summary>
		/// 
		/// </summary>
		public CommondType Type { get; } = CommondType.Delete;

		/// <summary>
		/// 
		/// </summary>
		public object? Data { get; } = null;
	}
}
