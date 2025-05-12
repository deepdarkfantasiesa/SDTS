using Auth.Domain.Events;
using Domain.Abstraction;

namespace Auth.Domain.AggregatesModel.UserAggregate
{
    public record UserId(Guid Value) : GuidEntityTypeId(Value);

    public class User : Entity<UserId>, IAggregateRoot
    {
        public User(ICollection<Address> address, string name)
        {
            Name = name;
            Address = address;
            AddDomainEvent(new CreateUserDomainEvent(this));
        }

        public User()
        {

        }

        public string Name { get; private set; }

        public string? Description { get; private set; }

        public virtual ICollection<UserRole> Roles { get; private set; }

        public virtual ICollection<Address> Address { get; private set; }

    }
}
