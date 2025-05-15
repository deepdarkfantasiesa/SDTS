using System.ComponentModel;

namespace Infrastructure.Core.Query
{
    /// <summary>
    /// 过滤操作枚举
    /// </summary>
    public enum ConditionOperator
    {
        /// <summary>
        /// 大于等于
        /// </summary>
        [Description("大于等于")]
        GreaterThanOrEqual = 0,

        /// <summary>
        /// 小于等于
        /// </summary>
        [Description("小于等于")]
        LessThanOrEqual = 1,

        /// <summary>
        /// 模糊匹配
        /// </summary>
        [Description("模糊匹配")]
        Like = 2,

        /// <summary>
        /// 等于
        /// </summary>
        [Description("等于")]
        Equal = 3,

        /// <summary>
        /// 不等于
        /// </summary>
        [Description("不等于")]
        NotEqual = 4,

        /// <summary>
        /// 大于
        /// </summary>
        [Description("大于")]
        GreaterThan = 5,

        /// <summary>
        /// 小于
        /// </summary>
        [Description("小于")]
        LessThan = 6,

        /// <summary>
        /// 在列表中
        /// </summary>
        [Description("在列表中")]
        In = 7,

        /// <summary>
        /// 不在列表中
        /// </summary>
        [Description("不在列表中")]
        NotIn = 8,

        /// <summary>
        /// 为空
        /// </summary>
        [Description("为空")]
        IsNull = 9,

        /// <summary>
        /// 不为空
        /// </summary>
        [Description("不为空")]
        IsNotNull = 10,
    }
}
