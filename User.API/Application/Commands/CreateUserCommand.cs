using MediatR;
using User.Domain.AggregatesModel.UserAggregate;

namespace User.API.Application.Commands
{
    public class CreateUserCommand:IRequest<bool>
    {
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
