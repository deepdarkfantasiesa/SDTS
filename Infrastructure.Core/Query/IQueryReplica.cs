
namespace Infrastructure.Core.Query
{
    /// <summary>
    /// query从库
    /// </summary>
    public interface IQueryReplica : MediatR.IRequest
    {
        /// <summary>
        /// 是否使用从库
        /// </summary>
        public bool UseReplica { get; init; }
    }

    public interface IQueryReplicaV2<out TResponse>: Domain.Abstraction.Mediator.IRequest<TResponse>
    {

    }
}
