namespace Auth.API.Application.Queries.Role.Page
{
    /// <summary>
    /// 返回数据
    /// </summary>
    public record QueryResult
    {
        public Guid Id { get; init; }

        public string Name { get; init; }

        public string Description { get; init; }

        public DateTime CreateAt { get; init; }

        public DateTime? UpdateAt { get; init; }
    }
}
