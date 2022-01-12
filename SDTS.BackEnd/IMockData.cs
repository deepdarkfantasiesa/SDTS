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
    }
}
