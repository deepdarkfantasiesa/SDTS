//using MediatR;

using Domain.Abstraction.Mediator;

namespace Domain.Abstraction
{
    /// <summary>
    /// 命令
    /// </summary>
    /// <typeparam name="TResponse">返回类型</typeparam>
    public interface ICommand<TResponse> : IRequest<TResponse>;
}
