using MediatR;
using System.Runtime.Serialization;

namespace Web.Auth.Application.Commands
{
    public class RegisterCommand:IRequest<bool>
    {
        public string Account { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }

        public RegisterCommand(string account,string password,int roleid)
        {
            Account = account;
            Password = password;
            RoleId = roleid;
        }
    }
}
