using Auth.Domain.AggregatesModel.RoleAggregate;
using Auth.Infrastructure.Repositories;
using Domain.Abstraction;

namespace Auth.API.Application.Commands.UserAggregate
{
    public record UpdateUserRoleCommand(Role Role) : ICommand<bool>;

    public class UpdateUserRoleCommandHandler(IUserRepo _userRepo) : ICommandHandler<UpdateUserRoleCommand, bool>
    {
        public async Task<bool> Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
        {
            var users = await _userRepo.GetByRoleIdAsync(request.Role.Id);

            if (users == null || !users.Any())
            {
                return true;
            }

            var userRoles = users.SelectMany(u => u.Roles)
                .Where(r => r.OriginalRoleId == request.Role.Id)
                .ToList();
            foreach (var userRole in userRoles)
            {
                userRole.Update(request.Role.Name, request.Role.Description);
            }

            var userPermissions = users
                .SelectMany(u => u.Roles)
                .SelectMany(r => r.Permissions)
                .ToList();

            foreach (var rolePermission in request.Role.Permissions)
            {
                var userPermission = userPermissions
                    .Where(p => p.OriginalPermissionId == rolePermission.Id)
                    .FirstOrDefault()
                    ?? throw new KeyNotFoundException($"未查询到id为{rolePermission.Id.ToString()}的用户权限");

                userPermission.Update(rolePermission.Name, rolePermission.Description, rolePermission.Url);
            }

            await _userRepo.UpdateRangeAsync(users, cancellationToken);

            return true;
        }
    }
}
