using Domain.Abstraction;
using User.Domain.AggregatesModel.UserAggregate;

namespace User.Domain.Events
{
    public class CreateUserDomainEvent : IDomainEvent
    {
        public Users User { get; private set; }
        public CreateUserDomainEvent(Users user)
        {
            User = user;
        }
    }
}
