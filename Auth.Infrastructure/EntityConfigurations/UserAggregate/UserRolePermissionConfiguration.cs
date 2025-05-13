using Auth.Domain.AggregatesModel.RoleAggregate;
using Auth.Domain.AggregatesModel.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auth.Infrastructure.EntityConfigurations.UserAggregate
{
    internal class UserRolePermissionConfiguration : IEntityTypeConfiguration<UserRolePermission>
    {
        public void Configure(EntityTypeBuilder<UserRolePermission> builder)
        {
            builder.ToTable("user_role_permission");

            // 配置主键
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .HasColumnName("id")
                .HasConversion(
                    id => id.Value, // 强类型 ID 转换为 Guid
                    value => new UserRolePermissionId(value)) // Guid 转换为强类型 ID
                .HasValueGenerator<StrongTypedIdValueGenerator<UserRolePermissionId>>();

            // 配置属性
            builder.Property(p => p.Name)
                .HasColumnName("name")
                .IsRequired()
                .HasMaxLength(100)
                .IsRequired(true);
            builder.Property(p => p.Description)
                .HasColumnName("description")
                .HasMaxLength(500)
                .IsRequired(false);
            builder.Property(p => p.Url)
                .HasColumnName("url")
                .HasMaxLength(200)
                .IsRequired(true);
            builder.Property(p => p.OriginalPermissionId)
                .HasColumnName("original_permission_id")
                .HasConversion(id =>
                    id.Value,
                    value => new RolePermissionId(value))
                .IsRequired(true);

            // 配置外键
            builder.Property(p => p.RoleId)
                .HasColumnName ("role_id")
                .HasConversion(
                    id => id.Value, // 强类型 ID 转换为 Guid
                    value => new UserRoleId(value)); // Guid 转换为强类型 ID

            builder.HasOne(p => p.Role)
                .WithMany(r => r.Permissions) // 配置 UserRole 的导航属性
                .HasForeignKey(p => p.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
