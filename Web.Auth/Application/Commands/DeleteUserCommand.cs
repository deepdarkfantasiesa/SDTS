using MediatR;

namespace Web.Auth.Application.Commands
{
    public class DeleteUserCommand:IRequest<bool>
    {
        public string Account { get; set; }
        public DeleteUserCommand(string account)
        {
            Account = account;
        }
        public DeleteUserCommand()
        {
            
        }
    }
}
