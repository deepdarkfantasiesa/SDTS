using Domain.Abstraction;

namespace Auth.API.Application.Commands.RoleAggregate
{
    public record DeleteRolePermissionCommand : ICommand<bool>
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid RolePermissionId { get; init; }
    }
}
