using Auth.Domain.AggregatesModel.RoleAggregate;
using Auth.Infrastructure.Repositories;
using Domain.Abstraction;
using Infrastructure.Core.Command;
using System.Data;

namespace Auth.API.Application.Commands.RoleAggregate
{
    public record UpdateRoleCommand : ICommand<bool>, ICommandTransactionSetting
    {
        public RoleId RoleId { get; init; }

        public string Name { get; init; }

        public string? Description { get; init; }

        public IEnumerable<UpdateRolePermission>? Permissions { get; init; }
        public IsolationLevel IsolationLevel { get; } = IsolationLevel.ReadCommitted;
        public int? Timeout { get; } = 1;
    }

    public record UpdateRolePermission
    {
        public RolePermissionId RolePermissionId { get; init; }

        public string Name { get; init; }

        public string? Description { get; init; }

        public string Url { get; init; }
    }

    public class UpdateRoleCommandHandler(IRoleRepo _roleRepo) : ICommandHandler<UpdateRoleCommand, bool>
    {
        public async Task<bool> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            //var roleId = new RoleId(request.RoleId);
            var role = await _roleRepo.GetByIdAsync(request.RoleId, cancellationToken)
                ?? throw new KeyNotFoundException($"未查询到id为{request.RoleId}的角色");

            foreach (var updatePermission in request.Permissions)
            {
                var permission = role.Permissions
                    .Where(p => p.Id == updatePermission.RolePermissionId)
                    .FirstOrDefault()
                    ?? throw new KeyNotFoundException($"未查询到id为{updatePermission.RolePermissionId}的权限");

                permission.Update(updatePermission.Name, updatePermission.Description, updatePermission.Url);
            }

            role.Update(request.Name, request.Description);

            await _roleRepo.UpdateAsync(role, cancellationToken);

            return true;
        }
    }
}
