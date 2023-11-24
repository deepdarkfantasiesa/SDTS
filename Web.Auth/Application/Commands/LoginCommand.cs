using MediatR;

namespace Web.Auth.Application.Commands
{
    public class LoginCommand:IRequest<string>
    {
        public string Account { get; set;}
        public string Password { get; set;}
        public LoginCommand(string account, string password)
        {
            Account = account;
            Password = password;
        }

        public LoginCommand()
        {
            
        }
    }
}
