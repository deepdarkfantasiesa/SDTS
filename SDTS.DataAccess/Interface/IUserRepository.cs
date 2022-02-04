using Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDTS.DataAccess.Interface
{
    public interface IUserRepository
    {
        public string SignUp(User user);
        public User SignIn(string account, string password);

        public User GetUser(string useraccount);
        public bool SignOut(string useraccount);
        public List<User> GetGuardians(string account);
        public string Invite(string account, string code);
        public List<User> GetWards(string account);
        public User GetWardDetail(string account);
    }
}
