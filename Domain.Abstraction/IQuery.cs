using MediatR;

namespace Domain.Abstraction
{
    /// <summary>
    /// 查询
    /// </summary>
    /// <typeparam name="TResponse">返回类型</typeparam>
    public interface IQuery<TResponse> : IRequest<TResponse> { }
}
