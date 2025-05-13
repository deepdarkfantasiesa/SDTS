using Auth.Domain.AggregatesModel.RoleAggregate;
using Domain.Abstraction;

namespace Auth.API.Application.Commands.UserAggregate
{
    public class UpdateUserRolePermissionCommand : ICommand<bool>
    {
        public RolePermissionId Id { get; init; }

        public string Name { get; init; }

        public string? Description { get; init; }

        public string Url { get; init; }
    }
}
