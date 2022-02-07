using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Models;
using SDTS.DataAccess.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDTS.DataAccess.Repository
{
    public class EmergencyHelpersRepository: IEmergencyHelpersRepository
    {
        private readonly SDTSContext _context;
        public EmergencyHelpersRepository(IServiceProvider provider)
        {
            _context = provider.CreateScope().ServiceProvider.GetRequiredService<SDTSContext>();
        }

        public async Task<bool> CreateEmergencyHelper(EmergencyHelper helper)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var table = await _context.EmergencyHelpers.ToListAsync();

                var selected = table.Where(p => p.Account == helper.Account).Count();
                if (selected != 0)
                {
                    return false;
                }
                var result = await _context.EmergencyHelpers.AddAsync(helper);
                if (result.State == EntityState.Added)
                {
                    await _context.SaveChangesAsync();
                }
                if (result.State == EntityState.Unchanged)
                {
                    transaction.Commit();
                    return true;
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return false;


            //var table = await _context.EmergencyHelpers.ToListAsync();

            //var selected = table.Where(p => p.Account == helper.Account).Count();
            //if(selected!=0)
            //{
            //    return false;
            //}
            //var result = await _context.EmergencyHelpers.AddAsync(helper);
            //if(result.State==EntityState.Added)
            //{
            //    await _context.SaveChangesAsync();
            //}
            //if(result.State==EntityState.Unchanged)
            //{
            //    return true;
            //}
            //return false;
        }

        public async Task<EmergencyHelper> QueryEmergencyHelper(string helperaccount)
        {
            var table = await _context.EmergencyHelpers.ToListAsync();
            var selected = table.Where(p => p.Account == helperaccount).FirstOrDefault();
            if(selected!=null)
            {
                return selected;
            }
            return null;
        }

        public async Task<bool> DeleteEmergencyHelper(string helperaccount)
        {
            var table = await _context.EmergencyHelpers.ToListAsync();
            var selected = table.Where(p => p.Account == helperaccount).FirstOrDefault();
            var deletehelper = _context.EmergencyHelpers.Attach(selected);
            if(deletehelper.State==EntityState.Unchanged)
            {
                _context.EmergencyHelpers.Remove(selected);
                if (deletehelper.State == EntityState.Deleted)
                {
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            return false ;
        }

        public async Task<List<EmergencyHelper>> GetAllEmergencyHelper()
        {
            return await _context.EmergencyHelpers.ToListAsync();
        }

    }
}
