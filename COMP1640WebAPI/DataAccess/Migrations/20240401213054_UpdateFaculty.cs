using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace COMP1640WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFaculty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "facultyId",
                table: "Contributions");

            migrationBuilder.AddColumn<string>(
                name: "facultyName",
                table: "Contributions",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "facultyName",
                table: "Contributions");

            migrationBuilder.AddColumn<int>(
                name: "facultyId",
                table: "Contributions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
