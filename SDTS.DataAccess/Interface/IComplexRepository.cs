using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDTS.DataAccess.Interface
{
    public interface IComplexRepository
    {
        public Task<bool> AddWard(string guardianaccount, string code);
        public Task<bool> RemoveWard(string guardianaccount,string wardaccount, string code);
    }
}
