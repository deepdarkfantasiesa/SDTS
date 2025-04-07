namespace User.Infrastructure.Caches.Models.SyncMemoryCacheCommds
{
    /// <summary>
    /// 创建命令
    /// </summary>
    public record CreateCommand<T> : BaseCommand<T>
	{
		public CommondType Type { get; } = CommondType.Create;
	}
}
