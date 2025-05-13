using Auth.Domain.Events;
using Domain.Abstraction;

namespace Auth.Domain.AggregatesModel.RoleAggregate
{
    public record RolePermissionId(Guid Value) : GuidEntityTypeId(Value);

    public class RolePermission : Entity<RolePermissionId>
    {
        public RolePermission()
        {

        }

        public RolePermission(string name, string? description, string url)
        {
            Name = name;
            Description = description;
            Url = url;
        }

        public void Update(string name, string? description, string url)
        {
            Name = name;
            Description = description;
            Url = url;
            UpdateAt = DateTime.UtcNow;
            AddDomainEvent(new UpdateRolePermissionDomainEvent
            {
                Id = Id,
                Description = description,
                Url = url,
                Name = name,
            });
        }

        /// <summary>
        /// 软删除
        /// </summary>
        public override void SoftDelete()
        {
            AddDomainEvent(new DeleteRolePermissionDomainEvent
            {
                RolePermissionId = Id
            });
            base.SoftDelete();
        }

        public string Name { get; private set; }

        public string? Description { get; private set; }

        public string Url { get; private set; }

        public RoleId RoleId { get; private set; }

        public virtual Role Role { get; private set; }
    }
}
