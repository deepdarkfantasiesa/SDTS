using MediatR;

namespace Web.Auth.Application.Commands
{
    public class LoginCommand:IRequest<string>
    {
        public string UserName { get; set;}
        public string Password { get; set;}
        public LoginCommand(string username, string password)
        {
            UserName= username;
            Password= password;
        }
    }
}
