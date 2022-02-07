using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDTS.DataAccess.Interface
{
    public interface IUserDataRepository
    {
        public Task<bool> AddUserDatasAsync(SensorsData userData);
        public Task<bool> AlterUserDatasAsync(SensorsData userData);
        public Task<bool> DeleteUserDatasAsync(string connectionid);
        public Task<SensorsData> QueryUserDatasAsync(string connectionid);
    }
}
