using Auth.Domain.Events;
using Domain.Abstraction;

namespace Auth.Domain.AggregatesModel.UserAggregate
{
    public record UserId(Guid Value) : IEntityTypeId<Guid>;

    public class User : Entity<UserId>, IAggregateRoot
    {
        public User(string name,string? description)
        {
            Name = name;
            Description = description;
            //AddDomainEvent(new CreateUserDomainEvent(this));
        }

        public User()
        {

        }

        public void AddRole(UserRole roles)
        {
            Roles.Add(roles);
        }

        public string Name { get; private set; }

        public string? Description { get; private set; }

        public virtual ICollection<UserRole> Roles { get; private set; } = [];

        public virtual ICollection<Address> Address { get; private set; } = [];

    }
}
