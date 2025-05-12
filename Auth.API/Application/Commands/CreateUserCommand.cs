using Domain.Abstraction;

namespace Auth.API.Application.Commands
{
    public class CreateUserCommand : ICommand<bool>
    {
        public string Name { get; init; }

        public string? Description { get; init; }

        public IEnumerable<CreateUserRoleSubCommand> Roles { get; init; }
        
    }

    public class CreateUserRoleSubCommand
    {
        public Guid Id { get; init; }

        public string Name { get; init; }

        public string? Description { get; init; }

        public IEnumerable<CreateUserRolePermissionSubCommand> Permissions { get; init; }
    }

    public class CreateUserRolePermissionSubCommand
    {
        public Guid Id { get; init; }

        public string Name { get; init; }

        public string? Description { get; init; }

        public string Url {  get; init; }
    }
}
