using System.ComponentModel;

namespace Infrastructure.Core
{
    /// <summary>
    /// redis槽枚举
    /// </summary>
    public enum RedisSlot
    {
        /// <summary>
        /// 用户服务
        /// </summary>
        [Description("用户服务")]
        UserService = 0
    }
}
