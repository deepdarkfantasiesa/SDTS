using Domain.Abstraction;

namespace Auth.API.Application.Commands.RoleAggregate
{
    /// <summary>
    /// 创建角色命令
    /// </summary>
    public class CreateRoleCommand : ICommand<bool>
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
}
