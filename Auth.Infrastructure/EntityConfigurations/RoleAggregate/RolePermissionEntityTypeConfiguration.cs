using Auth.Domain.AggregatesModel.RoleAggregate;
using Infrastructure.Core.Extension;
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
                .HasStronglyTypedIdConversion()
                .HasStronglyTypedIdValueGenerator();//设置自定义强类型id生成策略

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
                .HasStronglyTypedIdConversion();//设置强类型id转换规则

            builder.HasOne(p => p.Role)
                .WithMany(r => r.Permissions)
                .HasForeignKey(p => p.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
