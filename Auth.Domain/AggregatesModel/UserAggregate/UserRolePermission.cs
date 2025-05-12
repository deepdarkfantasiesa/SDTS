using Domain.Abstraction;

namespace Auth.Domain.AggregatesModel.UserAggregate
{
    public record UserRolePermissionId(Guid Value) : GuidEntityTypeId(Value);

    public class UserRolePermission : Entity<UserRolePermissionId>
    {
        public UserRolePermission()
        {

        }

        public UserRolePermission(Guid riginalPermissionId, string name, string? description, string url)
        {
            OriginalPermissionId = riginalPermissionId;
            Name = name; 
            Description = description; 
            Url = url;
        }

        public string Name { get; private set; }

        public string? Description { get; private set; }

        public string Url { get; private set; }

        public Guid OriginalPermissionId { get; private set; }

        public UserRoleId RoleId { get; private set; }

        public virtual UserRole Role { get; private set; }
    }
}
