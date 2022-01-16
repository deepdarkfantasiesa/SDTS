using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SDTS.BackEnd
{
    public interface IMockData
    {
        public User getdetail(int userid);
        public List<User> getwards(int userid);

        public List<SecureArea> getareas(int wardid);

        public bool addarea(SecureArea secureArea);
        public bool deletearea(SecureArea secureArea);
        public bool alterarea(SecureArea secureArea);
        public SecureArea getarea(int areaid);
        public User FindUser(string account, string password);
        public string signup(User user);
        public User getuser(string useraccount);
        public int invite(string account, int code);
        public List<User> getguardians(string account);
        public bool addward(int guardianid, int code);
        public bool AddConnectUser(string account, string connectid);
        public bool RemoveConnectUser(string account, string connectid);
    }
}
