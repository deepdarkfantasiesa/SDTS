namespace Infrastructure.Core.Query
{
    /// <summary>
    /// 过滤列标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ConditionColumnAttribute : Attribute
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string? TableName { get; }

        /// <summary>
        /// 列名
        /// </summary>
        public string ColumnName { get; }

        /// <summary>
        /// 过滤操作
        /// </summary>
        public ConditionOperator ConditionOperator { get; }

        /// <summary>
        /// 过滤列标签
        /// </summary>
        /// <param name="tableName">表名（如果有联表一定要填）</param>
        /// <param name="columnName">列名</param>
        /// <param name="conditionOperator">过滤操作</param>
        public ConditionColumnAttribute(string? tableName, string columnName, ConditionOperator conditionOperator)
        {
            TableName = tableName;
            ColumnName = columnName;
            ConditionOperator = conditionOperator;
        }
    }
}
