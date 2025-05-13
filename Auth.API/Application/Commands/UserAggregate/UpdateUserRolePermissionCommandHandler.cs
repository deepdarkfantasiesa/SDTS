using Auth.Infrastructure.Repositories;
using Domain.Abstraction;

namespace Auth.API.Application.Commands.UserAggregate
{
    public class UpdateUserRolePermissionCommandHandler(IUserRepo userRepo) : ICommandHandler<UpdateUserRolePermissionCommand, bool>
    {
        public async Task<bool> Handle(UpdateUserRolePermissionCommand request, CancellationToken cancellationToken)
        {
            var users = await userRepo.GetByRolePermissionIdAsync(request.Id);

            if (users == null || users.Count() == 0)
            {
                return true;
            }

            var permissions = users.SelectMany(u => u.Roles)
                .SelectMany(r => r.Permissions)
                .Where(p => p.OriginalPermissionId == request.Id)
                .ToList();

            foreach (var permission in permissions)
            {
                permission.Update(request.Name, request.Description, request.Url);
            }

            await userRepo.UpdateRangeAsync(users, cancellationToken);

            return true;
        }
    }
}
