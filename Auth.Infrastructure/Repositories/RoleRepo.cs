using Auth.Domain.AggregatesModel.RoleAggregate;
using Infrastructure.Core;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure.Repositories
{
    public interface IRoleRepo : IRepository<Role, RoleId>
    {
        Task<Role> GetByPermissionId(RolePermissionId id);
    }

    public class RoleRepo : Repository<UserContext, Role, RoleId>, IRoleRepo
    {
        private readonly UserContext _context;

        public RoleRepo(UserContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Role> GetByPermissionId(RolePermissionId id)
        {
            var role = await _context.Role
                .Where(p => p.Permissions
                    .Where(q => q.Id == id)
                    .Select(q => q.RoleId)
                    .First() == p.Id)
                .SingleAsync();

            return role;
        }
    }
}
