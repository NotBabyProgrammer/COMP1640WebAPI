using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace COMP1640WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateContributionsAca : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "acedemicYearId",
                table: "Contributions",
                newName: "academicYearId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "academicYearId",
                table: "Contributions",
                newName: "acedemicYearId");
        }
    }
}
