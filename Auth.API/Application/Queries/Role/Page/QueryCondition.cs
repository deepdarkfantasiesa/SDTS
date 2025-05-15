using Infrastructure.Core.Query;

namespace Auth.API.Application.Queries.Role.Page
{
    /// <summary>
    /// 筛选条件
    /// </summary>
    public record QueryCondition
    {
        [ConditionColumn("role", "description", ConditionOperator.Like)]
        public string? Description { get; init; }

        [ConditionColumn(null, "name", ConditionOperator.Like)]
        public string? Name { get; init; }

        [ConditionColumn(null, "create_at", ConditionOperator.GreaterThanOrEqual)]
        public DateTime? CreateAtStart { get; init; }

        [ConditionColumn(null, "create_at", ConditionOperator.LessThanOrEqual)]
        public DateTime? CreateAtEnd { get; init; }

        [ConditionColumn(null, "update_at", ConditionOperator.GreaterThanOrEqual)]
        public DateTime? UpdateAtStart { get; init; }

        [ConditionColumn(null, "update_at", ConditionOperator.LessThanOrEqual)]
        public DateTime? UpdateAtEnd { get; init; }
    }
}
