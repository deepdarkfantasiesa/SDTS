using MediatR;

namespace Infrastructure.Core.Query
{
    /// <summary>
    /// query从库
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    public interface IQueryReplica<TResponse> : IRequest<TResponse>
    {
        /// <summary>
        /// 是否使用从库
        /// </summary>
        public bool UseReplica { get; init; }
    }
}
