using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Models;
using SDTS.DataAccess.Interface;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SDTS.DataAccess.Repository
{
    public class ComplexRepository: IComplexRepository
    {
        private readonly SDTSContext _context;
        public ComplexRepository(IServiceProvider provider)
        {
            _context = provider.CreateScope().ServiceProvider.GetRequiredService<SDTSContext>();
        }

        public async Task<bool> AddWard(string guardianaccount, string code)
        {
            var invitation = _context.Invitations.Where(p => p.InviteCode == code).FirstOrDefaultAsync().Result;
            if (invitation == null)
            {
                return false;//邀请码不正确
            }
            var guardianandward = _context.GuardiansAndWards.Where(p => p.WardAccount == invitation.InviterAccount && p.GuardianAccount == guardianaccount).FirstOrDefaultAsync().Result;
            if (guardianandward != null)
            {
                return false;//已存在此被监护人
            }
            var newguardianandward = new GuardianAndWard() { GuardianAccount = guardianaccount, WardAccount = invitation.InviterAccount };
            await _context.GuardiansAndWards.AddAsync(newguardianandward);
            await _context.SaveChangesAsync();
            if (_context.GuardiansAndWards.Where(p => p.WardAccount == newguardianandward.WardAccount && p.GuardianAccount == newguardianandward.GuardianAccount).FirstOrDefaultAsync().Result != null)
            {
                _context.Invitations.Remove(invitation);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;

        }

        public async Task<bool> RemoveWard(string guardianaccount,string wardaccount, string code)
        {
            var invitation = _context.Invitations.Where(p => p.InviteCode == code&&p.InviterAccount==wardaccount).FirstOrDefaultAsync().Result;
            if (invitation == null)
            {
                return false;//邀请码不正确
            }
            var guardianandward = _context.GuardiansAndWards.Where(p => p.WardAccount == invitation.InviterAccount && p.GuardianAccount == guardianaccount).FirstOrDefaultAsync().Result;
            if (guardianandward == null)
            {
                return false;//不存在此被监护人
            }
            _context.GuardiansAndWards.Remove(guardianandward);
            await _context.SaveChangesAsync();
            if (_context.GuardiansAndWards.Where(p => p.WardAccount == guardianandward.WardAccount && p.GuardianAccount == guardianandward.GuardianAccount).FirstOrDefaultAsync().Result == null)
            {
                _context.Invitations.Remove(invitation);
                await _context.SaveChangesAsync();

                //此处后续加入删除对应监护人已创建的安全区域代码

                return true;
            }
            return false;
        }
    }
}
