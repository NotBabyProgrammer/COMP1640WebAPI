using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace COMP1640WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateContribution_AddAcademic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContributionsDates");

            migrationBuilder.DropTable(
                name: "Coordinators");

            migrationBuilder.DropTable(
                name: "GuessAccounts");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropColumn(
                name: "academicYearId",
                table: "Contributions");

            migrationBuilder.AddColumn<string>(
                name: "academic",
                table: "Contributions",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "academic",
                table: "Contributions");

            migrationBuilder.AddColumn<int>(
                name: "academicYearId",
                table: "Contributions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ContributionsDates",
                columns: table => new
                {
                    contributionsDateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    academicYearId = table.Column<int>(type: "int", nullable: false),
                    endDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    finalEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    startDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContributionsDates", x => x.contributionsDateId);
                });

            migrationBuilder.CreateTable(
                name: "Coordinators",
                columns: table => new
                {
                    coordinatorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    facultyId = table.Column<int>(type: "int", nullable: false),
                    userId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coordinators", x => x.coordinatorId);
                });

            migrationBuilder.CreateTable(
                name: "GuessAccounts",
                columns: table => new
                {
                    guestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    facultyId = table.Column<int>(type: "int", nullable: false),
                    guestName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    guestPassword = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuessAccounts", x => x.guestId);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    messageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    receiverId = table.Column<int>(type: "int", nullable: false),
                    senderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.messageId);
                });
        }
    }
}
