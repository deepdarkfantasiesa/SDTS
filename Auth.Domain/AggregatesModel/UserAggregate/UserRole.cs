using Auth.Domain.AggregatesModel.RoleAggregate;
using Domain.Abstraction;

namespace Auth.Domain.AggregatesModel.UserAggregate
{
    public record UserRoleId(Guid Value) : IEntityTypeId<Guid>;

    public class UserRole : Entity<UserRoleId>
    {
        public UserRole()
        {

        }

        public UserRole(RoleId originalRoleId, string name, string? description)
        {
            OriginalRoleId = originalRoleId;
            Name = name;
            Description = description;
        }

        public void AddPermission(UserRolePermission permission)
        {
            Permissions.Add(permission);
        }

        public void Update(string name, string? description)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; private set; }

        public string? Description { get; private set; }

        public UserId UserId { get; private set; }

        public virtual User User { get; private set; }

        public RoleId OriginalRoleId { get; private set; }

        public virtual ICollection<UserRolePermission> Permissions { get; private set; } = [];
    }
}
