using Infrastructure.Core.Query.Page;
using System.Text;

namespace Infrastructure.Core.Query
{
    /// <summary>
    /// StringBuilder拓展类
    /// </summary>
    public static class StringBuilderExtensions
    {
        /// <summary>
        /// 替换COLUMNClAUSE关键字
        /// </summary>
        /// <param name="stringBuilder"></param>
        /// <param name="columnSql"></param>
        /// <returns></returns>
        public static StringBuilder ReplaceColumn(this StringBuilder stringBuilder, ColumnSql columnSql)
        {
            stringBuilder.Replace("COLUMNClAUSE", columnSql.Value);
            return stringBuilder;
        }

        /// <summary>
        /// 替换CONDITIONClAUSE关键字
        /// </summary>
        /// <param name="stringBuilder"></param>
        /// <param name="conditionSql"></param>
        /// <returns></returns>
        public static StringBuilder ReplaceCondition(this StringBuilder stringBuilder, ConditionSql conditionSql)
        {
            stringBuilder.Replace("CONDITIONClAUSE", conditionSql.Value);
            return stringBuilder;
        }

        /// <summary>
        /// 替换PAGECLAUSE关键字
        /// </summary>
        /// <param name="stringBuilder"></param>
        /// <param name="pageSql"></param>
        /// <returns></returns>
        public static StringBuilder ReplacePage(this StringBuilder stringBuilder, PageSql pageSql)
        {
            stringBuilder.Replace("PAGECLAUSE", pageSql.Value);
            return stringBuilder;
        }

        /// <summary>
        /// 替换ORDERCLAUSE关键字
        /// </summary>
        /// <param name="stringBuilder"></param>
        /// <param name="orderSql"></param>
        /// <returns></returns>
        public static StringBuilder ReplaceOrder(this StringBuilder stringBuilder, OrderSql orderSql)
        {
            stringBuilder.Replace("ORDERCLAUSE", orderSql.Value);
            return stringBuilder;
        }
    }
}
