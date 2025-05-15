namespace Infrastructure.Core.Query
{
    /// <summary>
    /// 返回列映射标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SelectColumnAttribute : Attribute
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
        /// 返回列映射标签
        /// </summary>
        /// <param name="tableName">表名（如果有联表一定要填）</param>
        /// <param name="columnName">列名</param>
        public SelectColumnAttribute(string? tableName, string columnName)
        {
            TableName = tableName;
            ColumnName = columnName;
        }
    }
}
