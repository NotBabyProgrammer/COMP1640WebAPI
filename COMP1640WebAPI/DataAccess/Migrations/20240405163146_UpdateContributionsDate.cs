using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace COMP1640WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateContributionsDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_contributionsDates",
                table: "contributionsDates");

            migrationBuilder.RenameTable(
                name: "contributionsDates",
                newName: "ContributionsDates");

            migrationBuilder.AddColumn<int>(
                name: "academicYearId",
                table: "ContributionsDates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "acedemicYearId",
                table: "Contributions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContributionsDates",
                table: "ContributionsDates",
                column: "contributionsDateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ContributionsDates",
                table: "ContributionsDates");

            migrationBuilder.DropColumn(
                name: "academicYearId",
                table: "ContributionsDates");

            migrationBuilder.DropColumn(
                name: "acedemicYearId",
                table: "Contributions");

            migrationBuilder.RenameTable(
                name: "ContributionsDates",
                newName: "contributionsDates");

            migrationBuilder.AddPrimaryKey(
                name: "PK_contributionsDates",
                table: "contributionsDates",
                column: "contributionsDateId");
        }
    }
}
