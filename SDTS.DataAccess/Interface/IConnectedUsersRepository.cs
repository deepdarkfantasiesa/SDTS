using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDTS.DataAccess.Interface
{
    public interface IConnectedUsersRepository
    {
        public bool AddConnectUser(string account, string connectid);
        public bool RemoveConnectUser(string account, string connectid);
    }
}
