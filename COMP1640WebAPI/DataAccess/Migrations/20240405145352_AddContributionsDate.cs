using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace COMP1640WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddContributionsDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "closureDate",
                table: "Contributions",
                newName: "endDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "approvalDate",
                table: "Contributions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "contributionsDates",
                columns: table => new
                {
                    contributionsDateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    startDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    endDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    finalEndDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contributionsDates", x => x.contributionsDateId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contributionsDates");

            migrationBuilder.DropColumn(
                name: "approvalDate",
                table: "Contributions");

            migrationBuilder.RenameColumn(
                name: "endDate",
                table: "Contributions",
                newName: "closureDate");
        }
    }
}
