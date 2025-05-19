using Auth.Domain.AggregatesModel.RoleAggregate;
using Auth.Domain.AggregatesModel.UserAggregate;
using Auth.Infrastructure.Repositories;
using Domain.Abstraction;

namespace Auth.API.Application.Commands.UserAggregate
{
    public record CreateUserCommand : ICommand<bool>
    {
        public string Name { get; init; }

        public string? Description { get; init; }

        public IEnumerable<CreateUserRoleSubCommand> Roles { get; init; }

    }

    public record CreateUserRoleSubCommand
    {
        public RoleId Id { get; init; }

        public string Name { get; init; }

        public string? Description { get; init; }

        public IEnumerable<CreateUserRolePermissionSubCommand> Permissions { get; init; }
    }

    public record CreateUserRolePermissionSubCommand
    {
        public RolePermissionId Id { get; init; }

        public string Name { get; init; }

        public string? Description { get; init; }

        public string Url { get; init; }
    }

    public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, bool>
    {
        private readonly IUserRepo _repo;

        public CreateUserCommandHandler(IUserRepo userRepo)
        {
            _repo = userRepo;
        }

        public async Task<bool> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = new Domain.AggregatesModel.UserAggregate.User(request.Name, request.Description);

            foreach (var subRole in request.Roles)
            {
                var role = new UserRole(subRole.Id, subRole.Name, subRole.Description);

                foreach (var permission in subRole.Permissions)
                {
                    role.AddPermission(new UserRolePermission(permission.Id, permission.Name, permission.Description, permission.Url));
                }
                user.AddRole(role);
            }

            await _repo.AddAsync(user, cancellationToken);
            return true;
        }
    }
}
