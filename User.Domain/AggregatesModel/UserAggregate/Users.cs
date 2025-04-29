using Domain.Abstraction;
using System.ComponentModel;
using User.Domain.Events;

namespace User.Domain.AggregatesModel.UserAggregate
{
    public class UserIdTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string stringValue && Guid.TryParse(stringValue, out var guid))
            {
                return new UserId(guid);
            }

            throw new NotSupportedException("Invalid UserId format.");
        }
    }

    [TypeConverter(typeof(UserIdTypeConverter))]
    public record UserId : GuidEntityTypeId
    {
        public UserId(Guid id) : base(id)
        {

        }
    }

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
