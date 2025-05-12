using Domain.Abstraction;

namespace Auth.Domain.AggregatesModel.UserAggregate
{
    public record UserRolePermissionId(Guid Value) : GuidEntityTypeId(Value);

    public class UserRolePermission : Entity<UserRolePermissionId>
    {
        public UserRolePermission()
        {

        }

        public string Name { get; private set; }

        public string? Description { get; private set; }

        public string Url { get; private set; }

        public UserRoleId RoleId { get; private set; }

        public UserRole Role { get; private set; }
    }
}
