using Domain.Abstraction;
using User.Domain.Events;

namespace User.Domain.AggregatesModel.UserAggregate
{
    public record UserId(Guid Value) : GuidEntityTypeId(Value);

    public class Users : Entity<UserId>, IAggregateRoot
    {
        public Address Address { get; private set; }
        private string _name;

        public string Name => _name;

        public IEnumerable<int> GuardianIDs => guardianID;
        private readonly List<int> guardianID;

        public IEnumerable<string> EmergencyContacts => emergencyContacts;
        private readonly List<string> emergencyContacts;

        public Users(Address address, string? name = null)
        {
            _name = name;
            Address = address;
            AddDomainEvent(new CreateUserDomainEvent(this));
        }
        public Users()
        {

        }
    }
}
