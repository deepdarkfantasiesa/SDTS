using Auth.Domain.AggregatesModel.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auth.Infrastructure.EntityConfigurations
{
    public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("User");
            builder.HasKey(t => t.Id);
            builder.Property(p => p.Id)
                .HasConversion(p => p.Value, value => new UserId(value))
                .HasValueGenerator<StrongTypedIdValueGenerator<UserId>>();
            builder.OwnsMany(o => o.Address, a =>
            {
                a.WithOwner();
                a.Property(p => p.Country).HasMaxLength(50);
                a.Property(p => p.State).HasMaxLength(20);
                a.Property(p => p.City).HasMaxLength(20);
                a.Property(p => p.Street).HasMaxLength(30);
                a.Property(p => p.ZipCode).HasMaxLength(10);
            });

            builder.Property(p => p.Name).HasColumnName("name").IsRequired(true);
        }
    }
}
