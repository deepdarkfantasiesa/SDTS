using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Models;
using SDTS.DataAccess.Interface;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SDTS.DataAccess.Repository
{
    public class UserDataRepository: IUserDataRepository
    {
        private readonly SDTSContext _context;
        public UserDataRepository(IServiceProvider provider)
        {
            _context = provider.CreateScope().ServiceProvider.GetRequiredService<SDTSContext>();
        }

        public async Task<bool> AddUserDatasAsync(SensorsData userData)
        {
            var table = await _context.UserData.ToListAsync();
            var selected = table.Where(p => p.Account == userData.Account).FirstOrDefault();
            if(selected==null)
            {
                var result = await _context.UserData.AddAsync(userData);
                if (result.State == EntityState.Added)
                {
                    await _context.SaveChangesAsync();
                }
                if (result.State == EntityState.Unchanged)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> AlterUserDatasAsync(SensorsData userData)
        {
            var table = await _context.UserData.ToListAsync();
            var selected = table.FirstOrDefault(p => p.Account == userData.Account);
            if (selected != null)
            {
                var editdata = _context.UserData.Attach(selected);
                editdata.State = EntityState.Modified;
                selected.BarometerData = userData.BarometerData;
                selected.dateTime = userData.dateTime;
                selected.Latitude = userData.Latitude;
                selected.Longitude = userData.Longitude;
                selected.ConnectionId = userData.ConnectionId;
                await _context.SaveChangesAsync();
                if (editdata.State == EntityState.Unchanged)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> DeleteUserDatasAsync(string account)
        {
            var table = await _context.UserData.ToListAsync();
            var selected = table.FirstOrDefault(p => p.Account == account);
            if(selected!=null)
            {
                var editdata = _context.UserData.Attach(selected);
                _context.UserData.Remove(selected);
                if (editdata.State == EntityState.Deleted)
                {
                    await _context.SaveChangesAsync();
                }
                if (editdata.State == EntityState.Detached)
                {
                    return true;
                }
            }
            

            return false;
        }

        public async Task<SensorsData> QueryUserDatasAsync(string connectionid)
        {
            var table = await _context.UserData.ToListAsync();
            var selected = table.FirstOrDefault(p => p.ConnectionId == connectionid);
            if(selected!=null)
            {
                return selected;
            }
            return null;
        }

    }
}
