using System.ComponentModel;

namespace Infrastructure.Core
{
    /// <summary>
    /// 缓存等级枚举
    /// </summary>
    public enum CacheLevelEnum
    {
        /// <summary>
        /// 不使用缓存
        /// </summary>
        [Description("不使用缓存")]
        None = 0,

        /// <summary>
        /// 内存缓存（本地内存缓存）
        /// </summary>
        [Description("内存缓存（本地内存缓存）")]
        Memory = 1,

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
