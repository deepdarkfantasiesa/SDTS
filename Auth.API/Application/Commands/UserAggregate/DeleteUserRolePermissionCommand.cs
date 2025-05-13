using Auth.Domain.AggregatesModel.RoleAggregate;
using Domain.Abstraction;

namespace Auth.API.Application.Commands.UserAggregate
{
    public class DeleteUserRolePermissionCommand : ICommand<bool>
    {
        public RolePermissionId RolePermissionId { get; init; }
    }
}
