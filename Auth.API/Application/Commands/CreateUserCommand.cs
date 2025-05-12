using Domain.Abstraction;

namespace Auth.API.Application.Commands
{
    public class CreateUserCommand : ICommand<bool>
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
