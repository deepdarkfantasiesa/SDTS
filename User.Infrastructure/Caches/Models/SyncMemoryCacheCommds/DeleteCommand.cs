namespace User.Infrastructure.Caches.Models.SyncMemoryCacheCommds
{
	/// <summary>
	/// 
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
