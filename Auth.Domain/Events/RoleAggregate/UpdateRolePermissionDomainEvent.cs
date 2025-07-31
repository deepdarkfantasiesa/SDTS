using Auth.Domain.AggregatesModel.RoleAggregate;
using Domain.Abstraction;

namespace Auth.Domain.Events.RoleAggregate
{
    public sealed record UpdateRolePermissionDomainEvent(Role Role) : IDomainEvent;
}
