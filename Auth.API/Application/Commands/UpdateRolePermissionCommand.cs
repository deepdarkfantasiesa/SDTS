using Domain.Abstraction;

namespace Auth.API.Application.Commands
{
    public class UpdateRolePermissionCommand:ICommand<bool>
    {
        public Guid RolePermissionsId { get; init; }

        public string Name { get; init; }

        public string? Description { get; init; }

        public string Url { get;init; }
    }
}
