using Auth.Domain.AggregatesModel.RoleAggregate;
using Domain.Abstraction;

namespace Auth.Domain.AggregatesModel.UserAggregate
{
    public record UserRolePermissionId(Guid Value) : IEntityTypeId<Guid>;

    public class UserRolePermission : Entity<UserRolePermissionId>
    {
        public UserRolePermission()
        {

        }

        public UserRolePermission(RolePermissionId originalPermissionId, string name, string? description, string url)
        {
            OriginalPermissionId = originalPermissionId;
            Name = name;
            Description = description;
            Url = url;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="url"></param>
        public void Update(string name, string? description, string url)
        {
            Name = name;
            Description = description;
            Url = url;
            UpdateAt = DateTime.UtcNow;
        }

        public string Name { get; private set; }

        public string? Description { get; private set; }

        public string Url { get; private set; }

        public RolePermissionId OriginalPermissionId { get; private set; }

        public UserRoleId RoleId { get; private set; }

        public virtual UserRole Role { get; private set; }
    }
}
