using Auth.Infrastructure.Repositories;
using Domain.Abstraction;

namespace Auth.API.Application.Commands.UserAggregate
{
    public class DeleteUserRolePermissionCommandHandler(IUserRepo _userRepo) : ICommandHandler<DeleteUserRolePermissionCommand, bool>
    {
        public async Task<bool> Handle(DeleteUserRolePermissionCommand request, CancellationToken cancellationToken)
        {
            var users = await _userRepo.GetByRolePermissionIdAsync(request.RolePermissionId);

            if (users == null || users.Count() == 0)
            {
                return true;
            }

            var permissions = users
                .SelectMany(u => u.Roles)
                .SelectMany(r => r.Permissions)
                .Where(p => p.OriginalPermissionId == request.RolePermissionId)
                .ToList();

            foreach (var permission in permissions)
            {
                permission.SoftDelete();
            }

            await _userRepo.UpdateRangeAsync(users, cancellationToken);

            return true;
        }
    }
}
