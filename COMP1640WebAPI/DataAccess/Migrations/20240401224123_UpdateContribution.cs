using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace COMP1640WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateContribution : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "imagePath",
                table: "Contributions",
                newName: "imagePaths");

            migrationBuilder.RenameColumn(
                name: "filePath",
                table: "Contributions",
                newName: "filePaths");

            migrationBuilder.AddColumn<string>(
                name: "comments",
                table: "Contributions",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "comments",
                table: "Contributions");

            migrationBuilder.RenameColumn(
                name: "imagePaths",
                table: "Contributions",
                newName: "imagePath");

            migrationBuilder.RenameColumn(
                name: "filePaths",
                table: "Contributions",
                newName: "filePath");
        }
    }
}
