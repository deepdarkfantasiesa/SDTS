using Auth.Domain.AggregatesModel.RoleAggregate;
using Domain.Abstraction;

namespace Auth.Domain.Events
{
    public record DeleteRolePermissionDomainEvent : IDomainEvent
    {
        public RolePermissionId RolePermissionId { get; set; }
    }
}
