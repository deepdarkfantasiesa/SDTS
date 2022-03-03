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
    public class UserRepository: IUserRepository
    {
        private readonly SDTSContext _context;
        public UserRepository(IServiceProvider provider)
        {
            _context = provider.CreateScope().ServiceProvider.GetRequiredService<SDTSContext>();
        }

        public string SignUp(User user)
        {
            if (_context.Users.Where(p=>p.Account==user.Account).FirstOrDefault() != null)
            {
                return "账号已存在";
            }
            user.state = "signout";
            _context.Users.AddAsync(user);
            _context.SaveChangesAsync();
            if (_context.Users.Where(p => p.Account == user.Account) != null)
            {
                return "注册成功";
            }
            return "注册失败";
        }

        public User SignIn(string account, string password)
        {
            if(_context.Users.Where(p=>p.Account==account).FirstOrDefault()==null)
            {
                return null;//账号错误
            }
            if (_context.Users.Where(p => p.Account == account && p.PassWord == password).FirstOrDefault() == null)
            {
                return null;//密码错误
            }
            if (_context.Users.Where(p => p.Account == account).FirstOrDefault().state!="signout")
            {
                return null;//已经登录
            }
            var user = _context.Users.Where(p => p.Account == account && p.PassWord == password).FirstOrDefault();
            user.state = "signin";
            var edituser = _context.Users.Attach(user);
            edituser.State = EntityState.Modified;
            _context.SaveChanges();
            return _context.Users.Where(p => p.Account == account && p.PassWord == password).FirstOrDefault();
        }

        public User GetUser(string useraccount)
        {
            return _context.Users.Where(p => p.Account == useraccount).FirstOrDefault();
        }

        public async Task<bool> SignOut(string useraccount)
        {
            var table =await _context.Users.ToListAsync();
            var user = table.Where(p => p.Account == useraccount).FirstOrDefault();
            var edituser = _context.Users.Attach(user);
            edituser.State = EntityState.Modified;
            user.state = "signout";
            await _context.SaveChangesAsync();
            if(edituser.State==EntityState.Unchanged)
            {
                return true;
            }
            return false;
        }

        public async Task<List<User>> GetGuardiansAsync(string account)
        {
            var SelectedUsers =await _context.GuardiansAndWards.Where(p => p.WardAccount == account).ToListAsync();
            List<User> guardians = null;
            if (SelectedUsers != null)
            {
                guardians = new List<User>();
                foreach (var SelectedUser in SelectedUsers)
                {
                    var guardian = _context.Users.Where(p => p.Account == SelectedUser.GuardianAccount).Select(a => new User() { Name = a.Name, Information = a.Information, Account = a.Account, state = a.state }).FirstOrDefault();
                    guardians.Add(guardian);
                }
            }
            return guardians;
        }

        public List<User> GetWards(string account)
        {
            var SelectedUsers = _context.GuardiansAndWards.Where(p => p.GuardianAccount == account).ToList();
            List<User> Wards = null;
            if (SelectedUsers != null)
            {
                Wards = new List<User>();
                foreach (var SelectedUser in SelectedUsers)
                {
                    var ward = _context.Users.Where(p => p.Account == SelectedUser.WardAccount).Select(a => new User() { Name = a.Name, Information = a.Information, Account = a.Account, state = a.state }).FirstOrDefault();
                    Wards.Add(ward);
                }
            }
            return Wards;
        }

        public string Invite(string account, string code)
        {
            var UserInvitation = _context.Invitations.Where(p => p.InviterAccount == account).FirstOrDefault();
            if (UserInvitation != null)
            {
                UserInvitation.InviteCode = code;
                var edituser = _context.Invitations.Attach(UserInvitation);
                edituser.State = EntityState.Modified;
                _context.SaveChanges();
            }
            else
            {
                UserInvitation = new Invitation();
                UserInvitation.InviterAccount = account;
                UserInvitation.InviteCode = code;
                _context.Invitations.Add(UserInvitation);
                _context.SaveChanges();
            }

            var invitecode = _context.Invitations.Where(p => p.InviterAccount == account).FirstOrDefault().InviteCode;
            return invitecode;
        }

        public User GetWardDetail(string account)//目前与GetUser一样，计划后续更改
        {
            var ward = _context.Users.Where(p => p.Account == account).FirstOrDefault();
            return ward;
        }

        public async Task<List<User>> GetVolunteers()
        {
            var volunteers = _context.Users.Where(p => p.Type == "志愿者");
            return await volunteers.ToListAsync();
        }
    }
}
