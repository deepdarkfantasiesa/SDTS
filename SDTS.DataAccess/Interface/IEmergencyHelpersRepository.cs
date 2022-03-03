using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SDTS.DataAccess.Interface
{
    public interface IEmergencyHelpersRepository
    {
        public Task<bool> CreateEmergencyHelper(EmergencyHelper helper);

        public Task<EmergencyHelper> QueryEmergencyHelper(string helperaccount);
        public Task<bool> DeleteEmergencyHelper(string helperaccount);
        public Task<List<EmergencyHelper>> GetAllEmergencyHelper();
    }
}
