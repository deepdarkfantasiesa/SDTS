using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
