using Auth.Domain.AggregatesModel.RoleAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auth.Infrastructure.EntityConfigurations.RoleAggregate
{
    internal class RolePermissionEntityTypeConfiguration : IEntityTypeConfiguration<RolePermission>
    {
        public void Configure(EntityTypeBuilder<RolePermission> builder)
        {
            builder.ToTable("role_permission");

            builder.HasKey(t => t.Id);

            builder.Property(p => p.Id)
                .HasColumnName("id")
                .HasConversion(p => p.Value, value => new RolePermissionId(value))
                .HasValueGenerator<StrongTypedIdValueGenerator<RolePermissionId>>();

            builder.Property(p => p.Description)
                .HasColumnName("description")
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(p => p.Name)
                .HasColumnName("name")
                .HasMaxLength(100)
                .IsRequired(true);

            builder.Property(p => p.Url)
                .HasColumnName("url")
                .HasMaxLength(200)
                .IsRequired(true);

            builder.Property(p => p.RoleId)
                .HasColumnName("role_id")
                .HasConversion(
                    id => id.Value, // 强类型 ID 转换为 Guid
                    value => new RoleId(value));// Guid 转换为强类型 ID

            builder.HasOne(p => p.Role)
                .WithMany(r => r.Permissions)
                .HasForeignKey(p => p.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
