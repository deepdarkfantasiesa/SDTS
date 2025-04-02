using MediatR;

namespace Domain.Abstraction
{
    /// <summary>
    /// 领域事件
    /// </summary>
    public interface IDomainEvent : INotification { }
}
