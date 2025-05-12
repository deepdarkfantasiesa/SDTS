using Domain.Abstraction;

namespace Auth.Domain.Events
{
    public class UpdateRolePermissionDomainEvent(string name, string? description, string url) : IDomainEvent
    {
    }
}
