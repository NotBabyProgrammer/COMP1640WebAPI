using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace COMP1640WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class EditChatBox : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "userName",
                table: "ChatBoxes");

            migrationBuilder.AddColumn<int>(
                name: "userId",
                table: "ChatBoxes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "userId",
                table: "ChatBoxes");

            migrationBuilder.AddColumn<string>(
                name: "userName",
                table: "ChatBoxes",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
