﻿// <auto-generated />
using System;
using COMP1640WebAPI.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace COMP1640WebAPI.Migrations
{
    [DbContext(typeof(COMP1640WebAPIContext))]
    partial class COMP1640WebAPIContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("COMP1640WebAPI.DataAccess.Models.AcademicYears", b =>
                {
                    b.Property<int>("academicYearsId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("academicYearsId"));

                    b.Property<string>("academicYear")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("endDays")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("finalEndDays")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("startDays")
                        .HasColumnType("datetime2");

                    b.HasKey("academicYearsId");

                    b.ToTable("AcademicYears");
                });

            modelBuilder.Entity("COMP1640WebAPI.DataAccess.Models.Contributions", b =>
                {
                    b.Property<int>("contributionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("contributionId"));

                    b.Property<string>("academic")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("approval")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("approvalDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("commentions")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("endDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("facultyName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("filePaths")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("imagePaths")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("submissionDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("userId")
                        .HasColumnType("int");

                    b.HasKey("contributionId");

                    b.ToTable("Contributions");
                });

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

                    b.Property<string>("email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("facultyName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("notifications")
                        .HasColumnType("nvarchar(max)");

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
