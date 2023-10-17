using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Domain.AggregatesModel.UserAggregate;

namespace User.Domain.Events
{
    public class CreateUserDomainEvent: INotification
    {
        public Users User { get; private set; }
        public CreateUserDomainEvent(Users user)
        {
            User = user;
        }
    }
}
