using Auth.Infrastructure.Caches;

namespace Auth.Infrastructure.Caches.Models.SyncMemoryCacheCommds
{
    /// <summary>
    /// 删除命令
    /// </summary>
    public record DeleteCommand : BaseCommand
    {
        /// <summary>
        /// 键
        /// </summary>
        public override string Key { get; set; }

        /// <summary>
        /// 命令类型
        /// </summary>
        public override CommondType Type
        {
            get
            {
                return CommondType.Delete;
            }
            init
            {

            }
        }
    }
}
