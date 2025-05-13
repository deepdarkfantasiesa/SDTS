using Auth.Domain.AggregatesModel.RoleAggregate;
using Infrastructure.Core;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure.Repositories
{
    public interface IRoleRepo : IRepository<Role, RoleId>
    {
        /// <summary>
        /// 通过权限Id获取角色
        /// </summary>
        /// <param name="id">权限Id</param>
        /// <returns></returns>
        Task<Role> GetByPermissionIdAsync(RolePermissionId id);
    }

    public class RoleRepo : Repository<UserContext, Role, RoleId>, IRoleRepo
    {
        private readonly UserContext _context;

        public RoleRepo(UserContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// 通过权限Id获取角色
        /// </summary>
        /// <param name="id">权限Id</param>
        /// <returns></returns>
        public async Task<Role> GetByPermissionIdAsync(RolePermissionId id)
        {
            var role = await _context.Role
                .Where(p => p.Permissions
                    .Where(q => q.Id == id)
                    .Any())
                .SingleAsync();

            return role;
        }
    }
}
