using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace COMP1640WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAcademicYears : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "endYear",
                table: "AcademicYears");

            migrationBuilder.DropColumn(
                name: "startYear",
                table: "AcademicYears");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "endDay",
                table: "AcademicYears");

            migrationBuilder.DropColumn(
                name: "startDay",
                table: "AcademicYears");

            migrationBuilder.AddColumn<DateTime>(
                name: "endYear",
                table: "AcademicYears",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "startYear",
                table: "AcademicYears",
                type: "datetime2",
                nullable: true);
        }
    }
}
