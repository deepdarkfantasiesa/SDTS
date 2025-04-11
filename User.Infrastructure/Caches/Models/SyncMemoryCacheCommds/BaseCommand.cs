namespace User.Infrastructure.Caches.Models.SyncMemoryCacheCommds
{
    /// <summary>
    /// 基本命令
    /// </summary>
    public record BaseCommand
    {
        /// <summary>
        /// 键
        /// </summary>
        public virtual string Key { get; set; }

        /// <summary>
        /// 命令类型
        /// </summary>
        public virtual CommondType Type { get; init; }
    }
}
