using Auth.Domain.AggregatesModel.RoleAggregate;
using Auth.Infrastructure.Repositories;
using Domain.Abstraction;

namespace Auth.API.Application.Commands
{
    public class UpdateRolePermissionCommandHandler(IRoleRepo roleRepo) : ICommandHandler<UpdateRolePermissionCommand, bool>
    {
        public async Task<bool> Handle(UpdateRolePermissionCommand request, CancellationToken cancellationToken)
        {
            var rolePermissionId = new RolePermissionId(request.RolePermissionsId);
            var role = await roleRepo.GetByPermissionId(rolePermissionId);
            var permission = role.Permissions.Where(p => p.Id == rolePermissionId).First();
            permission.Update(request.Name, request.Description, request.Url);
            await roleRepo.UpdateAsync(role, false, cancellationToken);
            return true;
        }
    }
}
