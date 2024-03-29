﻿using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SDTS.DataAccess.Interface
{
    public interface IUserRepository
    {
        public string SignUp(User user);
        public User SignIn(string account, string password);

        public User GetUser(string useraccount);
        public Task<bool> SignOut(string useraccount);
        public Task<List<User>> GetGuardiansAsync(string account);
        public string Invite(string account, string code);
        public List<User> GetWards(string account);
        public User GetWardDetail(string account);
        public Task<List<User>> GetVolunteers();
    }
}
