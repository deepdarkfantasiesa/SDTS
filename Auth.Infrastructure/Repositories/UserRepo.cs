using Auth.Domain.AggregatesModel.RoleAggregate;
using Auth.Domain.AggregatesModel.UserAggregate;
using Infrastructure.Core;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure.Repositories
{
    public interface IUserRepo : IRepository<User, UserId>
    {
        Task<IEnumerable<User>> GetUserByRolePermissionId(RolePermissionId id);
    }

    public class UserRepo : Repository<UserContext, User, UserId>, IUserRepo
    {
        private readonly UserContext _context;

        public UserRepo(UserContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetUserByRolePermissionId(RolePermissionId id)
        {
            var users = await _context.Users.Where(p =>
                p.Roles.Where(q =>
                    q.Permissions.Where(o => o.OriginalPermissionId == id)
                    .Select(o => o.RoleId)
                    .First() == q.Id)
                .Select(q => q.UserId)
                .First() == p.Id)
                .ToListAsync();

            return users;
        }
    }
}
