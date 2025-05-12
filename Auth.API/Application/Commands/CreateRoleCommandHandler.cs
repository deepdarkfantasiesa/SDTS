using Auth.Domain.AggregatesModel.RoleAggregate;
using Auth.Infrastructure.Repositories;
using Domain.Abstraction;

namespace Auth.API.Application.Commands
{
    public class CreateRoleCommandHandler(IRoleRepo roleRepo) : ICommandHandler<CreateRoleCommand, bool>
    {

        public async Task<bool> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            var role = new Role(request.Name, request.Description);

            role.AddPermissions(request.Permissions.Select(p => new RolePermission(p.Name, p.Description, p.Url)));

            await roleRepo.AddAsync(role, cancellationToken);

            return true;
        }
    }
}
