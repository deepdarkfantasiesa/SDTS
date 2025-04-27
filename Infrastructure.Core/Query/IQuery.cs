using MediatR;

namespace Infrastructure.Core.Query
{
    /// <summary>
    /// query
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    public interface IQuery<TResponse> : IRequest<TResponse>
    {
    }
}
