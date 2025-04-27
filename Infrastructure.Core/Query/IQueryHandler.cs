using MediatR;

namespace Infrastructure.Core.Query
{
    /// <summary>
    /// 查询处理者
    /// </summary>
    /// <typeparam name="TRequest">查询类型</typeparam>
    /// <typeparam name="TResponse">返回类型</typeparam>
    public interface IQueryHandler<TRequest, TResponse>
        : IRequestHandler<TRequest, TResponse> where TRequest : IQuery<TResponse>
    {
    }
}
