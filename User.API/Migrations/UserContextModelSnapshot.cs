﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using User.Infrastructure;

#nullable disable

namespace User.API.Migrations
{
    [DbContext(typeof(UserContext))]
    partial class UserContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("User.Domain.AggregatesModel.UserAggregate.Users", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("_name")
                        .HasColumnType("longtext")
                        .HasColumnName("Name");

                    b.HasKey("Id");

                    b.ToTable("User", (string)null);
                });

            modelBuilder.Entity("User.Domain.AggregatesModel.UserAggregate.Users", b =>
                {
                    b.OwnsOne("User.Domain.AggregatesModel.UserAggregate.Address", "Address", b1 =>
                        {
                            b1.Property<int>("UsersId")
                                .HasColumnType("int");

                            b1.Property<string>("City")
                                .IsRequired()
                                .HasMaxLength(20)
                                .HasColumnType("varchar(20)");

                            b1.Property<string>("Country")
                                .IsRequired()
                                .HasMaxLength(50)
                                .HasColumnType("varchar(50)");

                            b1.Property<string>("State")
                                .IsRequired()
                                .HasMaxLength(20)
                                .HasColumnType("varchar(20)");

                            b1.Property<string>("Street")
                                .IsRequired()
                                .HasMaxLength(30)
                                .HasColumnType("varchar(30)");

                            b1.Property<string>("ZipCode")
                                .IsRequired()
                                .HasMaxLength(10)
                                .HasColumnType("varchar(10)");

                            b1.HasKey("UsersId");

                            b1.ToTable("User");

                            b1.WithOwner()
                                .HasForeignKey("UsersId");
                        });

                    b.Navigation("Address")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}