using Auth.Domain.AggregatesModel.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Infrastructure.Core.Extension;

namespace Auth.Infrastructure.EntityConfigurations.UserAggregate
{
    internal class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("user");

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
                .IsRequired(true);

            builder.OwnsMany(o => o.Address, a =>
            {
                a.WithOwner();
                a.Property(p => p.Country).HasMaxLength(50);
                a.Property(p => p.State).HasMaxLength(20);
                a.Property(p => p.City).HasMaxLength(20);
                a.Property(p => p.Street).HasMaxLength(30);
                a.Property(p => p.ZipCode).HasMaxLength(10);
            });

        }
    }
}
