using MediatR;

namespace Web.Auth.Application.Commands
{
    public class RegisterCommand:IRequest<bool>
    {
        public string Account { get; set; }
        public string Password { get; set; }

        public RegisterCommand(string account,string password)
        {
            Account = account;
            Password = password;
        }
    }
}
