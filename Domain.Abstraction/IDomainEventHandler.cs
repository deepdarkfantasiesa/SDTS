using MediatR;

//using Domain.Abstraction.Mediator;

namespace Domain.Abstraction
{
    /// <summary>
    /// 领域事件处理者
    /// </summary>
    public interface IDomainEventHandler<TRequest> : INotificationHandler<TRequest>
        where TRequest : IDomainEvent
    {
    }
}
