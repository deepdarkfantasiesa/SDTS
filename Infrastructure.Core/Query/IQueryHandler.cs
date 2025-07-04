using Infrastructure.Core.DatabaseContext;
using MediatR;

//using Domain.Abstraction.Mediator;

namespace Infrastructure.Core.Query
{
    /// <summary>
    /// 查询处理者
    /// </summary>
    /// <typeparam name="TRequest">查询类型</typeparam>
    /// <typeparam name="TParams">参数类型</typeparam>
    /// <typeparam name="TResponse">返回类型</typeparam>
    public interface IQueryHandler<TRequest, TParams, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IQueryBase<TParams, TResponse>
        where TParams : IQueryParam
    {
        /// <summary>
        /// 查询上下文
        /// </summary>
        public IQueryDbContext _queryContext { get; }
    }
}
