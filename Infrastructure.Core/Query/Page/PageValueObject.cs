namespace Infrastructure.Core.Query.Page
{
    /// <summary>
    /// 分页sql
    /// </summary>
    public record PageSql
    {
        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// 分页sql
        /// </summary>
        /// <param name="sql">sql语句</param>
        public PageSql(string sql)
        {
            Value = sql;
        }
    }

    /// <summary>
    /// 列sql
    /// </summary>
    public record ColumnSql
    {
        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// 列sql
        /// </summary>
        /// <param name="sql">sql语句</param>
        public ColumnSql(string sql)
        {
            Value = sql;
        }
    }

    /// <summary>
    /// 排序sql
    /// </summary>
    public record OrderSql
    {
        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// 排序sql
        /// </summary>
        /// <param name="sql">sql语句</param>
        public OrderSql(string sql)
        {
            Value= sql;
        }
    }

    /// <summary>
    /// 条件sql
    /// </summary>
    public record ConditionSql
    {
        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// 条件sql
        /// </summary>
        /// <param name="sql">sql语句</param>
        public ConditionSql(string sql)
        {
            Value = sql;
        }
    }

    /// <summary>
    /// 分组sql
    /// </summary>
    public record GroupSql
    {
        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// 分组sql
        /// </summary>
        /// <param name="sql">sql语句</param>
        public GroupSql(string sql)
        {
            Value = sql;
        }
    }
}
