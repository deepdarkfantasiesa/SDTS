using Auth.Domain.AggregatesModel.RoleAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Infrastructure.Core.Extension;

namespace Auth.Infrastructure.EntityConfigurations.RoleAggregate
{
    internal class RoleEntityTypeConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("role");

            builder.HasKey(t => t.Id);

            builder.Property(p => p.Id)
                .HasColumnName("id")
                .HasStronglyTypedIdConversion()//设置强类型id转换规则
                .HasStronglyTypedIdValueGenerator();//设置自定义强类型id生成策略

            builder.Property(p => p.Description)
                .HasColumnName("description")
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(p => p.Name)
                .HasColumnName("name")
                .HasMaxLength(100)
                .IsRequired(true);

            builder.HasMany(p => p.Permissions)
                .WithOne(p => p.Role)
                .HasForeignKey(p => p.RoleId);
        }
    }
}
