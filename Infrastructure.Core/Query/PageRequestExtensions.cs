using System.Reflection;

namespace Infrastructure.Core.Query
{
    /// <summary>
    /// 分页入参拓展方法
    /// </summary>
    public static class PageRequestExtensions
    {
        /// <summary>
        /// 生成排序sql
        /// </summary>
        /// <typeparam name="TCondition"></typeparam>
        /// <param name="pageRequest"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static string GetSortClause<TCondition>(this PageRequest<TCondition> pageRequest)
        {
            if (pageRequest.Sorts == null || !pageRequest.Sorts.Any())
            {
                return "ORDER BY create_at DESC ";
            }

            var sortClauses = new List<string>();
            var properties = typeof(TCondition).GetProperties();

            foreach (var sort in pageRequest.Sorts)
            {
                var property = properties.FirstOrDefault(p => p.Name == sort.SortName);
                if (property == null)
                {
                    throw new InvalidOperationException($"Property '{sort.SortName}' does not exist on type '{typeof(TCondition).Name}'.");
                }

                var attribute = property.GetCustomAttribute<DatabaseMapAttribute>();
                if (attribute == null)
                {
                    throw new InvalidOperationException($"Property '{sort.SortName}' does not have a ColumnNameAttribute.");
                }

                var columnName = string.IsNullOrWhiteSpace(attribute.TableName)
                    ? attribute.ColumnName
                    : $"{attribute.TableName}.{attribute.ColumnName}";
                var direction = sort.IsAsc ? "ASC" : "DESC";
                sortClauses.Add($"{columnName} {direction}");
            }

            return "ORDER BY " + string.Join(", ", sortClauses) + " ";
        }

        /// <summary>
        /// 生成分组条件sql
        /// </summary>
        /// <typeparam name="TCondition"></typeparam>
        /// <param name="pageRequest"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static string GetGroupSql<TCondition>(this PageRequest<TCondition> pageRequest)
        {
            if (pageRequest.GroupBy == null || pageRequest.GroupBy.Count() == 0)
            {
                return "";
            }

            var groupConditions = new List<string>();
            var properties = typeof(TCondition).GetProperties();

            foreach (var group in pageRequest.GroupBy)
            {
                var property = properties.FirstOrDefault(p => p.Name == group);
                if (property == null)
                {
                    throw new InvalidOperationException($"Property '{group}' does not exist on type '{typeof(TCondition).Name}'.");
                }

                var attribute = property.GetCustomAttribute<DatabaseMapAttribute>();
                if (attribute == null)
                {
                    throw new InvalidOperationException($"Property '{group}' does not have a ColumnNameAttribute.");
                }

                var columnName = string.IsNullOrWhiteSpace(attribute.TableName)
                    ? attribute.ColumnName
                    : $"{attribute.TableName}.{attribute.ColumnName}";
                groupConditions.Add(columnName);
            }
            return $"GROUP BY {string.Join(", ", groupConditions)} ";
        }
    }
}
