using Infrastructure.Core.Query;

namespace Auth.API.Application.Queries.Role.Page
{
    /// <summary>
    /// 返回数据
    /// </summary>
    public record QueryResult
    {
        [SelectColumn(null, "id")]
        public Guid Id { get; init; }

        [SelectColumn("role", "name")]
        public string Name { get; init; }

        [SelectColumn(null, "description")]
        public string Description { get; init; }

        [SelectColumn("role", "create_at")]
        public DateTime CreateAt { get; init; }

        [SelectColumn(null, "update_at")]
        public DateTime? UpdateAt { get; init; }
    }
}
