using Auth.Domain.AggregatesModel.RoleAggregate;
using Auth.Infrastructure.Repositories;
using Domain.Abstraction;

namespace Auth.API.Application.Commands.RoleAggregate
{
    public class CreateRoleCommandHandler(IRoleRepo roleRepo) : ICommandHandler<CreateRoleCommand, bool>
    {
        public async Task<bool> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            //实例化角色
            var role = new Role(request.Name, request.Description);

            //批量插入权限
            role.AddPermissions(request.Permissions.Select(p => new RolePermission(p.Name, p.Description, p.Url)));

            //插入数据库
            await roleRepo.AddAsync(role, cancellationToken);

            return true;
        }
    }
}
