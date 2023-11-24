using MediatR;

namespace Web.Auth.Application.Commands
{
    public class CreateRoleCommand:IRequest<bool>
    {
        public string Name { get; set; }
        public CreateRoleCommand(string name)
        {
            Name = name;
        }
        public CreateRoleCommand()
        {
            
        }
    }
}
