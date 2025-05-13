using Auth.Domain.AggregatesModel.RoleAggregate;
using Auth.Infrastructure.Repositories;
using Domain.Abstraction;

namespace Auth.API.Application.Commands.RoleAggregate
{
    public class DeleteRolePermissionCommandHandler(IRoleRepo _roleRepo) : ICommandHandler<DeleteRolePermissionCommand, bool>
    {
        public async Task<bool> Handle(DeleteRolePermissionCommand request, CancellationToken cancellationToken)
        {
            var permissionId = new RolePermissionId(request.RolePermissionId);
            var role = await _roleRepo.GetByPermissionIdAsync(permissionId);
            var permission = role.Permissions.Where(p => p.Id == permissionId).First();
            permission.SoftDelete();
            await _roleRepo.UpdateAsync(role);
            return true;
        }
    }
}
