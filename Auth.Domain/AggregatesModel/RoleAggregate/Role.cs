using Auth.Domain.Events.RoleAggregate;
using Domain.Abstraction;

namespace Auth.Domain.AggregatesModel.RoleAggregate
{
    public record RoleId(Guid Value) : IEntityTypeId<Guid>;

    public class Role : AggregateRoot<RoleId>
    {
        public Role()
        {

        }

        public Role(string name, string? description)
        {
            Name = name;
            Description = description;
        }

        /// <summary>
        /// 批量插入权限
        /// </summary>
        /// <param name="permissions">权限列表</param>
        public void AddPermissions(IEnumerable<RolePermission> permissions)
        {
            Permissions = permissions.ToList();
        }

        public void Update(string name, string? description)
        {
            Name = name;
            Description = description;
            AddDomainEvent(new UpdateRolePermissionDomainEvent(this));
        }

        public string Name { get; private set; }

        public string? Description { get; private set; }

        public virtual ICollection<RolePermission> Permissions { get; private set; }
    }
}
