using MediatR;

namespace Infrastructure.Core.Query
{
    /// <summary>
    /// query从库
    /// </summary>
    public interface IQueryReplica : IRequest
    {
        /// <summary>
        /// 是否使用从库
        /// </summary>
        public bool UseReplica { get; init; }
    }
}
