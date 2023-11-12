using MediatR;
using System.Runtime.Serialization;
using User.Domain.AggregatesModel.UserAggregate;

namespace User.API.Application.Commands
{
    public class CreateUserCommand:IRequest<bool>
    {
        //[DataMember]
        public string UserName { get; private set; }

        public CreateUserCommand(string name)
        {
            UserName = name;
        }

        public CreateUserCommand()
        {

        }
    }
}
