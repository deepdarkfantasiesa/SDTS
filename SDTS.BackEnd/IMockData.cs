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

    }
}
