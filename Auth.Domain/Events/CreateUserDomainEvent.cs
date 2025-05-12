using Domain.Abstraction;

namespace Auth.Domain.Events
{
    public class CreateUserDomainEvent : IDomainEvent
    {
        public AggregatesModel.UserAggregate.User User { get; private set; }
        public CreateUserDomainEvent(AggregatesModel.UserAggregate.User user)
        {
            User = user;
        }
    }
}
