namespace User.Infrastructure.Caches.Models.SyncMemoryCacheCommds
{
    /// <summary>
    /// 删除命令
    /// </summary>
    public record DeleteByKeyCommand<T> : BaseCommand<T>
	{
		/// <summary>
		/// 
		/// </summary>
		public CommondType Type { get; } = CommondType.DeleteByKey;

		/// <summary>
		/// 
		/// </summary>
		public object? Data { get; } = null;
	}
}
