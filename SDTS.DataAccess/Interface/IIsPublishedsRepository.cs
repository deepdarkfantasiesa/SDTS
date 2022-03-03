using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SDTS.DataAccess.Interface
{
    public interface IIsPublishedsRepository
    {
        public Task<bool> AddUser(IsPublished user);
        public Task<List<IsPublished>> QueryUsersAsync(string helperaccount);
        public Task DeleteUsers(string helperaccount);

    }
}
