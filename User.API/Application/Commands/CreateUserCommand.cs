using Domain.Abstraction;

namespace User.API.Application.Commands
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
