﻿using System.Threading.Tasks;

namespace SDTS.DataAccess.Interface
{
    public interface IConnectedUsersRepository
    {
        public Task<bool> AddConnectUser(string account, string connectid);
        public Task<bool> RemoveConnectUser(string account, string connectid);
        public Task<string> QueryConnectUserAsync(string account);
    }
}
