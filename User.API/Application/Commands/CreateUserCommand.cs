using MediatR;
using User.Domain.AggregatesModel.UserAggregate;

namespace User.API.Application.Commands
{
    public class CreateUserCommand:IRequest<bool>
    {
        private readonly Users _user;
        public CreateUserCommand()
        {

        }
    }
}
