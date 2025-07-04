using MediatR;

//using Domain.Abstraction.Mediator;

namespace Domain.Abstraction
{
    /// <summary>
    /// 命令处理者
    /// </summary>
    /// <typeparam name="TRequest">命令类型</typeparam>
    /// <typeparam name="Tresponse">返回类型</typeparam>
    public interface ICommandHandler<TRequest, Tresponse> : IRequestHandler<TRequest, Tresponse> 
        where TRequest : ICommand<Tresponse>
    {
    }
}
