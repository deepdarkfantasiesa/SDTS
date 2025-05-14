using Infrastructure.Core.Query;

namespace Auth.API.Application.Queries.Role.Page
{
    /// <summary>
    /// 筛选条件
    /// </summary>
    public record QueryCondition
    {
        [DatabaseMap("role", "description")]
        public string? Description { get; init; }

        [DatabaseMap(null, "name")]
        public string? Name { get; init; }

        [DatabaseMap(null, "create_at")]
        public DateTime? CreateAtStart { get; init; }

        [DatabaseMap(null, "create_at")]
        public DateTime? CreateAtEnd { get; init; }

        [DatabaseMap(null, "update_at")]
        public DateTime? UpdateAtStart { get; init; }

        [DatabaseMap(null, "update_at")]
        public DateTime? UpdateAtEnd { get; init; }
    }
}
