using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Models;
using SDTS.DataAccess.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDTS.DataAccess.Repository
{
    public class RescureGroupRepository: IRescureGroupRepository
    {
        private readonly SDTSContext _context;
        public RescureGroupRepository(IServiceProvider provider)
        {
            _context = provider.CreateScope().ServiceProvider.GetRequiredService<SDTSContext>();
        }

        public async Task<bool> AddToRescureGroup(RescureGroup rescureGroup)
        {
            var table =await _context.RescureGroups.ToListAsync();
            if(table.Exists(p=>p.Account==rescureGroup.Account)==false)
            {
                var result = await _context.RescureGroups.AddAsync(rescureGroup);
                if(result.State==EntityState.Added)
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
        public async Task<RescureGroup> QueryRescurer(string rescureraccount)
        {
            var table = await _context.RescureGroups.ToListAsync();
            var selected = table.Where(p => p.Account == rescureraccount).FirstOrDefault();
            return selected;

        }

        public async Task<bool> DeleteRescurerAsync(string rescureraccount)
        {
            var table = await _context.RescureGroups.ToListAsync();
            var selected = table.Where(p => p.Account == rescureraccount).FirstOrDefault();
            var deletedata = _context.RescureGroups.Attach(selected);
            if(deletedata.State==EntityState.Unchanged)
            {
                _context.RescureGroups.Remove(selected);
                if(deletedata.State==EntityState.Deleted)
                {
                    await _context.SaveChangesAsync();
                    if(deletedata.State==EntityState.Detached)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public async Task<IEnumerable<RescureGroup>> DeleteRescurersAsync(string groupname)
        {
            var table = await _context.RescureGroups.ToListAsync();
            var selected = table.Where(p => p.GroupName == groupname);
            _context.RescureGroups.RemoveRange(selected);
            await _context.SaveChangesAsync();
            return selected;
        }
    }
}
