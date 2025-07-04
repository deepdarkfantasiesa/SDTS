using System.ComponentModel;

namespace Infrastructure.Core.Cache
{
    /// <summary>
    /// 缓存等级
    /// </summary>
    public enum CacheLevel
    {
        /// <summary>
        /// 本地内存缓存
        /// </summary>
        [Description("本地内存缓存")]
        Local = 1,

        /// <summary>
        /// 分布式缓存（Redis）
        /// </summary>
        [Description("分布式缓存（Redis）")]
        Distributed = 2,

        ///// <summary>
        ///// 持久化缓存（数据库或文件系统）
        ///// </summary>
        //[Description("持久化缓存（数据库或文件系统）")]
        //Persistent = 3,
    }
}
