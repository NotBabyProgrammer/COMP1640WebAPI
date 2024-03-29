﻿// <auto-generated />
using COMP1640WebAPI.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace COMP1640WebAPI.Migrations
{
    [DbContext(typeof(COMP1640WebAPIContext))]
    [Migration("20240309213754_I4")]
    partial class I4
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("COMP1640WebAPI.DataAccess.Models.Faculties", b =>
                {
                    b.Property<int>("facultyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("facultyId"));

                    b.Property<string>("facultyName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("facultyId");

                    b.ToTable("Faculties");
                });

            modelBuilder.Entity("COMP1640WebAPI.DataAccess.Models.GuessAccounts", b =>
                {
                    b.Property<int>("guestId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("guestId"));

                    b.Property<int>("facultyId")
                        .HasColumnType("int");

                    b.Property<string>("guestName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("guestPassword")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("guestId");

                    b.ToTable("GuessAccounts");
                });

            modelBuilder.Entity("COMP1640WebAPI.DataAccess.Models.Roles", b =>
                {
                    b.Property<int>("roleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("roleId"));

                    b.Property<string>("roleName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("roleId");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("COMP1640WebAPI.DataAccess.Models.Users", b =>
                {
                    b.Property<int>("userId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("userId"));

                    b.Property<string>("password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("roleId")
                        .HasColumnType("int");

                    b.Property<string>("userName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("userId");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
