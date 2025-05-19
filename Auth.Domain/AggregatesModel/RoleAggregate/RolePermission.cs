using Domain.Abstraction;

namespace Auth.Domain.AggregatesModel.RoleAggregate
{
    public record RolePermissionId(Guid Value) : IEntityTypeId<Guid>;

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
        }

        public string Name { get; private set; }

        public string? Description { get; private set; }

        public string Url { get; private set; }

        public RoleId RoleId { get; private set; }

        public virtual Role Role { get; private set; }
    }
}
