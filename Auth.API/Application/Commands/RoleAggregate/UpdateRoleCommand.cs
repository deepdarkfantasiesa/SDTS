using Auth.Domain.AggregatesModel.RoleAggregate;
using Auth.Infrastructure.Repositories;
using Domain.Abstraction;

namespace Auth.API.Application.Commands.RoleAggregate
{
    public record UpdateRoleCommand:ICommand<bool>
    {
        public Guid RoleId { get; init; }

        public string Name { get; init; }

        public string? Description {  get; init; }

        public IEnumerable<UpdateRolePermission>? Permissions { get; init; }
    }

    public record UpdateRolePermission
    {
        public Guid RolePermissionId { get; init; }

        public string Name { get; init; }

        public string? Description { get; init; }

        public string Url {  get; init; }
    }

    public class UpdateRoleCommandHandler(IRoleRepo _roleRepo) : ICommandHandler<UpdateRoleCommand, bool>
    {
        public async Task<bool> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var roleId = new RoleId(request.RoleId);
            var role = await _roleRepo.GetByIdAsync(roleId, cancellationToken) 
                ?? throw new KeyNotFoundException($"未查询到id为{request.RoleId}的角色");

            foreach(var updatePermission in request.Permissions)
            {
                var permission = role.Permissions
                    .Where(p => p.Id == new RolePermissionId(updatePermission.RolePermissionId))
                    .FirstOrDefault()
                    ?? throw new KeyNotFoundException($"未查询到id为{updatePermission.RolePermissionId}的权限");

                permission.Update(updatePermission.Name, updatePermission.Description, updatePermission.Url);
            }

            role.Update(request.Name, request.Description);

            await _roleRepo.UpdateAsync(role,cancellationToken);

            return true;
        }
    }
}
