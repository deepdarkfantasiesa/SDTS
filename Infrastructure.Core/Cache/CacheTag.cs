using System.ComponentModel;

namespace Infrastructure.Core.Cache
{
    /// <summary>
    /// 缓存标签
    /// </summary>
    public enum CacheTag
    {
        /// <summary>
        /// 后台任务
        /// </summary>
        [Description("后台任务")]
        Background = 0,

        /// <summary>
        /// 健康检查
        /// </summary>
        [Description("健康检查")]
        HealthCheck = 1,

        /// <summary>
        /// 关系型数据库
        /// </summary>
        [Description("关系型数据库")]
        RelationDatabaseConfig = 2,

        /// <summary>
        /// 查询
        /// </summary>
        [Description("查询")]
        Query = 3,

        /// <summary>
        /// 检查是否存在
        /// </summary>
        [Description("检查是否存在")]
        CheckIsExist = 4,

        /// <summary>
        /// 分页
        /// </summary>
        [Description("分页")]
        Page = 5
    }
}
