namespace Infrastructure.Core.Query
{
    /// <summary>
    /// 数据库映射标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DatabaseMapAttribute : Attribute
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
        /// 数据库映射标签
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="columnName">列名</param>
        public DatabaseMapAttribute(string? tableName, string columnName)
        {
            TableName = tableName;
            ColumnName = columnName;
        }
    }
}
