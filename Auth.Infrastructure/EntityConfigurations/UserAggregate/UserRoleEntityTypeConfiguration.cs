using Auth.Domain.AggregatesModel.UserAggregate;
using Infrastructure.Core.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auth.Infrastructure.EntityConfigurations.UserAggregate
{
    internal class UserRoleEntityTypeConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.ToTable("user_role");

            // 配置主键
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id)
                .HasColumnName("id")
                .HasStronglyTypedIdConversion()//设置强类型id转换规则
                .HasStronglyTypedIdValueGenerator();//设置自定义强类型id生成策略

            // 配置属性
            builder.Property(r => r.Name)
                .IsRequired()
                .HasColumnName("name")
                .HasMaxLength(100)
                .IsRequired(true);
            builder.Property(r => r.Description)
                .HasColumnName("description")
                .HasMaxLength(500)
                .IsRequired(false);
            builder.Property(r => r.OriginalRoleId)
                .HasColumnName("original_role_id")
                .HasStronglyTypedIdConversion()//设置强类型id转换规则
                .IsRequired(true);

            // 配置外键
            builder.Property(r => r.UserId)
                .HasColumnName("user_id")
                .HasStronglyTypedIdConversion();//设置强类型id转换规则

            builder.HasOne(r => r.User)
                .WithMany(u => u.Roles) // 配置 User 的导航属性
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // 配置导航属性
            builder.HasMany(r => r.Permissions)
                .WithOne(p => p.Role) // 配置 UserRolePermission 的导航属性
                .HasForeignKey(p => p.RoleId) // 配置外键
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
