using Domain.Abstraction;

namespace Auth.Domain.AggregatesModel.UserAggregate
{
    public record UserRoleId(Guid Value) : GuidEntityTypeId(Value);

    public class UserRole : Entity<UserRoleId>
    {
        public UserRole()
        {
            
        }

        public string Name {  get; private set; }

        public string? Description { get; private set; }

        public UserId UserId { get; private set; }

        public User User { get; private set; }

        public virtual ICollection<UserRolePermission> Permissions { get; private set; }
    }
}
