using Auth.Domain.AggregatesModel.RoleAggregate;
using Domain.Abstraction;

namespace Auth.Domain.Events.RoleAggregate
{
    public record UpdateRolePermissionDomainEvent(Role Role) : IDomainEvent;
}
