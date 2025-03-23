using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameNest_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddUserNameToPublication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Publications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Publications");
        }
    }
}
