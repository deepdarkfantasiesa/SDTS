using Domain.Abstraction;

namespace Auth.API.Application.Commands
{
    public class CreateRoleCommand:ICommand<bool>
    {
        public string Name { get; init; }

        public string? Description { get; init; }

        public IEnumerable<RolePermissionSubCommand> Permissions { get; init; }
    }

    public record RolePermissionSubCommand
    {
        public string Name { get; init; }

        public string? Description { get; init; }

        public string Url { get; init; }
    }
}
