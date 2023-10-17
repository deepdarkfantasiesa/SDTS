using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Domain.AggregatesModel.UserAggregate;

namespace User.Infrastructure.EntityConfigurations
{
    public class UserEntityTypeConfiguration : IEntityTypeConfiguration<Users>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Users> builder)
        {
            builder.HasKey(t => t.Id);
            builder.ToTable("User");
            builder.OwnsOne(o => o.Address, a =>
            {
                a.WithOwner();
                a.Property(p => p.Country).HasMaxLength(50);
                a.Property(p => p.State).HasMaxLength(20);
                a.Property(p => p.City).HasMaxLength(20);
                a.Property(p => p.Street).HasMaxLength(50);
                a.Property(p => p.ZipCode).HasMaxLength(10);
            });
        }
    }
}
