using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stardust.Migrations
{
    /// <inheritdoc />
    public partial class RemoveJira : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JiraMail",
                table: "StardustUsers");

            migrationBuilder.DropColumn(
                name: "JiraToken",
                table: "StardustUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JiraMail",
                table: "StardustUsers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JiraToken",
                table: "StardustUsers",
                type: "TEXT",
                nullable: true);
        }
    }
}
