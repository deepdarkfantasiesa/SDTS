using Domain.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Domain.Events;

namespace User.Domain.AggregatesModel.UserAggregate
{
    public class Users:Entity,IAggregateRoot
    {
        public Address Address { get; private set; }
        private string _name;

        public IEnumerable<int> GuardianIDs => guardianID;
        private readonly List<int> guardianID;

        public IEnumerable<string> EmergencyContacts => emergencyContacts;
        private readonly List<string> emergencyContacts;

        public Users(Address address,string? name=null)
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
