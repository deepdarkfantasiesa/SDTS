namespace Infrastructure.Core.Query.Page
{
    /// <summary>
    /// 排序选项
    /// </summary>
    public record SortOption
    {
        /// <summary>
        /// 排序字段
        /// </summary>
        public string SortName { get; init; }

        /// <summary>
        /// 是否升序：true升序、false降序
        /// </summary>
        public bool IsAsc { get; init; }
    }

    /// <summary>
    /// 分页查询请求
    /// </summary>
    /// <typeparam name="TCondition">查询条件</typeparam>
    public record PageRequest<TCondition> : IQueryParam
    {
        /// <summary>
        /// 筛选条件
        /// </summary>
        public TCondition? Conditions { get; init; }

        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageNumber { get; init; } = 1;

        /// <summary>
        /// 每页大小
        /// </summary>
        public int PageSize { get; init; } = 10;

        /// <summary>
        /// 排序
        /// </summary>
        public IEnumerable<SortOption>? Sorts { get; init; }

        /// <summary>
        /// 分组
        /// </summary>
        public IEnumerable<string>? GroupBy { get; init; }
    }

    /// <summary>
    /// 分页查询响应
    /// </summary>
    /// <typeparam name="TData">响应数据</typeparam>
    public record PageResponse<TData>
    {
        /// <summary>
        /// 当前页的数据列表
        /// </summary>
        public IEnumerable<TData>? Items { get; set; }

        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// 每页大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        /// <summary>
        /// 分页查询响应
        /// </summary>
        public PageResponse()
        {

        }

        /// <summary>
        /// 分页查询响应
        /// </summary>
        /// <param name="items">当前页的数据列表</param>
        /// <param name="pageNumber">当前页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="totalCount">总记录数</param>
        public PageResponse(IEnumerable<TData> items, int pageNumber, int pageSize, int totalCount)
        {
            Items = items;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = totalCount;
        }
    }
}
