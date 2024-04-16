using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace COMP1640WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAcademicYears2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "endDay",
                table: "AcademicYears");

            migrationBuilder.DropColumn(
                name: "startDay",
                table: "AcademicYears");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "AcademicYears",
                newName: "academicYearsId");

            migrationBuilder.AddColumn<DateTime>(
                name: "endDays",
                table: "AcademicYears",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "finalEndDays",
                table: "AcademicYears",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "startDays",
                table: "AcademicYears",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "endDays",
                table: "AcademicYears");

            migrationBuilder.DropColumn(
                name: "finalEndDays",
                table: "AcademicYears");

            migrationBuilder.DropColumn(
                name: "startDays",
                table: "AcademicYears");

            migrationBuilder.RenameColumn(
                name: "academicYearsId",
                table: "AcademicYears",
                newName: "Id");

            migrationBuilder.AddColumn<DateOnly>(
                name: "endDay",
                table: "AcademicYears",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "startDay",
                table: "AcademicYears",
                type: "date",
                nullable: true);
        }
    }
}
