using System.Reflection;

namespace Infrastructure.Core.Query.Page
{
    /// <summary>
    /// 分页查询拓展方法
    /// </summary>
    public static class PageExtensions
    {
        /// <summary>
        /// 生成排序sql
        /// </summary>
        /// <typeparam name="TCondition"></typeparam>
        /// <param name="pageRequest"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static OrderSql GetOrderClause<TCondition>(this PageRequest<TCondition> pageRequest)
        {
            if (pageRequest.Sorts == null || !pageRequest.Sorts.Any())
            {
                return new OrderSql("ORDER BY create_at DESC");
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

                var attribute = property.GetCustomAttribute<ConditionColumnAttribute>();
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

            return new OrderSql("ORDER BY " + string.Join(", ", sortClauses));
        }

        /// <summary>
        /// 生成分组条件sql
        /// </summary>
        /// <typeparam name="TCondition"></typeparam>
        /// <param name="pageRequest"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static GroupSql GetGroupClause<TCondition>(this PageRequest<TCondition> pageRequest)
        {
            if (pageRequest.GroupBy == null || pageRequest.GroupBy.Count() == 0)
            {
                return new GroupSql("");
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

                var attribute = property.GetCustomAttribute<SelectColumnAttribute>();
                if (attribute == null)
                {
                    throw new InvalidOperationException($"Property '{group}' does not have a ColumnNameAttribute.");
                }

                var columnName = string.IsNullOrWhiteSpace(attribute.TableName)
                    ? attribute.ColumnName
                    : $"{attribute.TableName}.{attribute.ColumnName}";
                groupConditions.Add(columnName);
            }
            return new GroupSql($"GROUP BY {string.Join(", ", groupConditions)}");
        }

        /// <summary>
        /// 生成查询列sql
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="pageResponse"></param>
        /// <returns></returns>
        public static ColumnSql GetColumnClause<TData>(this PageResponse<TData> pageResponse)
        {
            //获取所有应用了SelectColumnAttribute的属性
            var properties = typeof(TData).GetProperties()
                .Where(p => p.GetCustomAttributes(typeof(SelectColumnAttribute), false).Any())
                .ToList() ?? throw new ArgumentNullException("请为需要返回的列打上'SelectColumnAttribute'标签");

            var selectClauses = new List<string>();

            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttribute<SelectColumnAttribute>();

                var selectColumn = string.IsNullOrWhiteSpace(attribute.TableName)
                    ? $"{attribute.ColumnName} AS {property.Name}"
                    : $"{attribute.TableName}.{attribute.ColumnName} AS {property.Name}";

                selectClauses.Add(selectColumn);
            }

            return new ColumnSql(string.Join(",", selectClauses));
        }

        /// <summary>
        /// 获取接在“Where is_deleted=FALSE”之后的其他筛选条件
        /// </summary>
        /// <typeparam name="TCondition"></typeparam>
        /// <param name="pageRequest"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        public static ConditionSql GetConditionClause<TCondition>(this PageRequest<TCondition> pageRequest, ref Dictionary<string, object> parameters)
        {
            //获取所有应用了ConditionColumnAttribute的属性
            var properties = typeof(TCondition).GetProperties()
                .Where(p => p.GetCustomAttributes(typeof(ConditionColumnAttribute), false).Any())
                .ToList() ?? throw new ArgumentNullException("请为需要返回的列打上'ConditionColumnAttribute'标签");

            var conditions = new List<string>();

            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttribute<ConditionColumnAttribute>();

                var value = property.GetValue(pageRequest.Conditions);

                if (value == null)
                    continue;

                var column = string.IsNullOrEmpty(attribute.TableName)
                    ? attribute.ColumnName
                    : $"{attribute.TableName}.{attribute.ColumnName}";

                string conditionSql = attribute.ConditionOperator switch
                {
                    ConditionOperator.Equal => $"{column} = @{property.Name}",
                    ConditionOperator.NotEqual => $"{column} != @{property.Name}",
                    ConditionOperator.GreaterThan => $"{column} > @{property.Name}",
                    ConditionOperator.GreaterThanOrEqual => $"{column} >= @{property.Name}",
                    ConditionOperator.LessThan => $"{column} < @{property.Name}",
                    ConditionOperator.LessThanOrEqual => $"{column} <= @{property.Name}",
                    ConditionOperator.Like => $"{column} LIKE '%' || @{property.Name} || '%'",
                    ConditionOperator.In => $"{column} IN (@{property.Name})",
                    ConditionOperator.NotIn => $"{column} NOT IN (@{property.Name})",
                    ConditionOperator.IsNull => $"{column} IS NULL",
                    ConditionOperator.IsNotNull => $"{column} IS NOT NULL",
                    _ => throw new NotSupportedException($"Operator {attribute.ConditionOperator} is not supported.")
                };

                conditions.Add(conditionSql);
                parameters.Add(property.Name, value);
            }

            var conditionClause = conditions.Any() ? " AND " + string.Join(" AND ", conditions) : string.Empty;
            return new ConditionSql(conditionClause);
        }

        /// <summary>
        /// 获取分页sql
        /// </summary>
        /// <typeparam name="TCondition"></typeparam>
        /// <param name="pageRequest"></param>
        /// <returns></returns>
        public static PageSql GetPageClause<TCondition>(this PageRequest<TCondition> pageRequest, ref Dictionary<string, object> parameters)
        {
            parameters.Add("PageNumber", pageRequest.PageNumber);
            parameters.Add("PageSize", pageRequest.PageSize);

            return new PageSql("LIMIT @PageSize OFFSET (@PageNumber - 1) * @PageSize");
        }

        /// <summary>
        /// 获取缓存键上下文
        /// </summary>
        /// <typeparam name="TCondition"></typeparam>
        /// <param name="pageRequest"></param>
        /// <returns></returns>
        public static CacheKeyContext GetCacheKeyContext<TCondition>(this PageRequest<TCondition> pageRequest)
        {
            var keyContext = new CacheKeyContext();

            keyContext.Add("PageNumber", pageRequest.PageNumber);
            keyContext.Add("PageSize", pageRequest.PageSize);

            if (pageRequest.Sorts != null && pageRequest.Sorts.Any())
            {
                int i = 0;
                foreach (var sort in pageRequest.Sorts)
                {
                    keyContext.Add($"SortBy{sort.SortName}{i++}", sort.IsAsc);
                }
            }

            if (pageRequest.GroupBy != null && pageRequest.GroupBy.Any())
            {
                int i = 0;
                foreach (var group in pageRequest.GroupBy)
                {
                    keyContext.Add($"Group{i++}", group);
                }
            }

            var properties = typeof(TCondition).GetProperties();
            foreach (var property in properties)
            {
                var value = property.GetValue(pageRequest.Conditions);
                if (value == null)
                    continue;

                keyContext.Add(property.Name, value);
            }

            return keyContext;
        }
    }
}
