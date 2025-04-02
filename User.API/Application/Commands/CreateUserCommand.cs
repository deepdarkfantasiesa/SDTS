using Domain.Abstraction;

namespace User.API.Application.Commands
{
    public class CreateUserCommand : ICommand<bool>
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
