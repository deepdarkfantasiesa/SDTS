using Auth.Domain.AggregatesModel.RoleAggregate;
using Auth.Domain.AggregatesModel.UserAggregate;
using Infrastructure.Core;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure.Repositories
{
    public interface IUserRepo : IRepository<User, UserId>
    {
        Task<IEnumerable<User>> GetByRolePermissionIdAsync(RolePermissionId id);
    }

    public class UserRepo : Repository<UserContext, User, UserId>, IUserRepo
    {
        private readonly UserContext _context;

        public UserRepo(UserContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetByRolePermissionIdAsync(RolePermissionId id)
        {
            var users = await _context.Users
                .Where(p => p.Roles
                    .SelectMany(q => q.Permissions)
                    .Where(q => q.OriginalPermissionId == id)
                    .Any())
                .ToListAsync();

            return users;
        }
    }
}
