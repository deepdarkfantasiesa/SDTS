﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Models;
using SDTS.DataAccess.Interface;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SDTS.DataAccess.Repository
{
    public class ConnectedUsersRepository: IConnectedUsersRepository
    {
        private readonly SDTSContext _context;
        public ConnectedUsersRepository(IServiceProvider provider)
        {
            _context = provider.CreateScope().ServiceProvider.GetRequiredService<SDTSContext>();
        }

        public async Task<bool> AddConnectUser(string account,string connectid)
        {
            var table = await _context.ConnectedUsers.ToListAsync();
            var selected = table.Where(p => p.Account == account).FirstOrDefault();
            if (selected != null)
            {
                var editdata = _context.ConnectedUsers.Attach(selected);
                editdata.State = EntityState.Modified;
                selected.ConnectId = connectid;
                await _context.SaveChangesAsync();
                if(editdata.State==EntityState.Unchanged)
                {
                    return true;
                }
            }
            await _context.ConnectedUsers.AddAsync(new ConnectedUser() { Account = account, ConnectId = connectid });
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveConnectUser(string account, string connectid)
        {
            var table =await _context.ConnectedUsers.ToListAsync();
            var selected = table.Where(p => p.Account == account).FirstOrDefault();
            var editdata = _context.ConnectedUsers.Attach(selected);
            _context.ConnectedUsers.Remove(selected);
            await _context.SaveChangesAsync();
            if(editdata.State==EntityState.Detached)
            {
                return true;
            }
            return false;
        }

        public async Task<string> QueryConnectUserAsync(string account)
        {
            var table =await _context.ConnectedUsers.ToListAsync();
            try
            {
                if(table.Where(p=>p.Account==account).FirstOrDefault()!=null)
                {
                    return table.Where(p => p.Account == account).FirstOrDefault().ConnectId;
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
           
            return null;
        }
    }
}
