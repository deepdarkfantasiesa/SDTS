using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Models;
using SDTS.DataAccess.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SDTS.DataAccess.Repository
{
    public class SecureAreaRepository: ISecureAreaRepository
    {
        private readonly SDTSContext _context;
        public SecureAreaRepository(IServiceProvider provider)
        {
            _context = provider.CreateScope().ServiceProvider.GetRequiredService<SDTSContext>();
        }

        public async Task<bool> AddareaAsync(SecureArea secureArea)
        {
            await _context.SecureAreas.AddAsync(secureArea);
            await _context.SaveChangesAsync();
            if (_context.SecureAreas.Where(p => p.areaid == secureArea.areaid).FirstOrDefaultAsync().Result != null)
            {
                return true;
            }
            return false;
        }

        public async Task<SecureArea> FindareaAsync(string secureAreaid)
        {
            var areas = await _context.SecureAreas.ToListAsync();
            var targetarea = areas.Where(p => p.areaid == secureAreaid).FirstOrDefault();
            if (targetarea != null)
            {
                return targetarea;
            }
            return null;
        }

        public async Task<SecureArea> AlterareaAsync(SecureArea secureArea)
        {
            var oldarea =await FindareaAsync(secureArea.areaid);
            if(oldarea!=null)
            {
                oldarea.creatername = secureArea.creatername;
                oldarea.createraccount = secureArea.createraccount;
                oldarea.createtime = secureArea.createtime;
                oldarea.status = secureArea.status;
                oldarea.information = secureArea.information;
                var editarea = _context.SecureAreas.Attach(oldarea);
                editarea.State = EntityState.Modified;
                await _context.SaveChangesAsync();
                var newarea = await FindareaAsync(secureArea.areaid);
                if(newarea!=null)
                {
                    return newarea;
                }
            }

            return null;
        }

        public async Task<bool> DeleteareaAsync(string secureAreaid)
        {
            var deletearea = await FindareaAsync(secureAreaid);
            _context.SecureAreas.Remove(deletearea);
            await _context.SaveChangesAsync();
            var deletedarea = await FindareaAsync(secureAreaid);
            if (deletedarea==null)
            {
                return true;
            }
            return false;
        }

        public async Task<List<SecureArea>> FindWardAreasAsync(string wardaccount)
        {
            var allareas =await _context.SecureAreas.ToListAsync();
            var selectareas = allareas.FindAll(p => p.wardaccount == wardaccount);
            return selectareas;
        }
    }
}
