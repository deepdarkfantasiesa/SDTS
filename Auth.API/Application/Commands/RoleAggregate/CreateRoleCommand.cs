using Auth.Domain.AggregatesModel.RoleAggregate;
using Auth.Infrastructure.Repositories;
using Domain.Abstraction;
using Infrastructure.Core.Command;
using System.Data;

namespace Auth.API.Application.Commands.RoleAggregate
{
    /// <summary>
    /// 创建角色命令
    /// </summary>
    public record CreateRoleCommand : ICommand<bool>, ICommandTransactionSetting
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// 描述
        /// </summary>
        public string? Description { get; init; }

        /// <summary>
        /// 权限列表
        /// </summary>
        public IEnumerable<RolePermissionSubCommand> Permissions { get; init; }

        public IsolationLevel IsolationLevel { get; set; } = IsolationLevel.ReadCommitted;

        public int? Timeout { get; set; } = 5;
    }

    /// <summary>
    /// 权限
    /// </summary>
    public record RolePermissionSubCommand
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// 描述
        /// </summary>
        public string? Description { get; init; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Url { get; init; }
    }

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
