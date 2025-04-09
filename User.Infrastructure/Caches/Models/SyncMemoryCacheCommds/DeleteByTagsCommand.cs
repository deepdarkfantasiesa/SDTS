namespace User.Infrastructure.Caches.Models.SyncMemoryCacheCommds
{
    /// <summary>
    /// 删除命令
    /// </summary>
    public record DeleteByTagsCommand<T> : BaseCommand<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public CommondType Type { get; } = CommondType.DeleteByTags;
    }
}
