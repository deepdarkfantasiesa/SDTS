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
    public class IsPublishedsRepository: IIsPublishedsRepository
    {
        private readonly SDTSContext _context;
        public IsPublishedsRepository(IServiceProvider provider)
        {
            _context = provider.CreateScope().ServiceProvider.GetRequiredService<SDTSContext>();
        }

        public async Task<bool> AddUser(IsPublished user)
        {
            var result = await _context.IsPublisheds.AddAsync(user);
            if(result.State==EntityState.Added)
            {
                await _context.SaveChangesAsync();
            }
            if(result.State == EntityState.Unchanged)
            {
                return true;
            }
            return false;

        }

        public async Task<List<IsPublished>> QueryUsersAsync(string helperaccount)
        {
            var selected = _context.IsPublisheds.Where(p => p.HelperAccount == helperaccount);

            var users = await selected.ToListAsync();

            return users;
        }

        public async Task DeleteUsers(string helperaccount)
        {
            var table = await _context.IsPublisheds.ToListAsync();
            var selected = table.Where(p => p.HelperAccount == helperaccount);
            _context.IsPublisheds.RemoveRange(selected);
            await _context.SaveChangesAsync();
        }
    }
}
