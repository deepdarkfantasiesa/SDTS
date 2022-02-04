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
    public class ConnectedUsersRepository: IConnectedUsersRepository
    {
        private readonly SDTSContext _context;
        public ConnectedUsersRepository(IServiceProvider provider)
        {
            _context = provider.CreateScope().ServiceProvider.GetRequiredService<SDTSContext>();
        }

        public bool AddConnectUser(string account,string connectid)
        {
            if(_context.ConnectedUsers.Where(p=>p.Account==account).FirstOrDefault()!=null)
            {
                return false;//已连接
            }
            _context.ConnectedUsers.AddAsync(new ConnectedUser() { Account=account,ConnectId=connectid});
            _context.SaveChangesAsync();
            return true;
        }

        public bool RemoveConnectUser(string account, string connectid)
        {
            var user = _context.ConnectedUsers.Where(p => p.Account == account && p.ConnectId == connectid).FirstOrDefault();
            if (user == null)
            {
                return false;
            }
            _context.ConnectedUsers.Remove(user);
            _context.SaveChangesAsync();
            if (_context.ConnectedUsers.Where(p => p.Account == account && p.ConnectId == connectid).FirstOrDefault()==null)
            {
                return true;
            }
            return false;
        }
    }
}
