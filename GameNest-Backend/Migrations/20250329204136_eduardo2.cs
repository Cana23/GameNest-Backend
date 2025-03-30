using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameNest_Backend.Migrations
{
    /// <inheritdoc />
    public partial class eduardo2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Publications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHidden",
                table: "Publications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastEditedDate",
                table: "Publications",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Publications");

            migrationBuilder.DropColumn(
                name: "IsHidden",
                table: "Publications");

            migrationBuilder.DropColumn(
                name: "LastEditedDate",
                table: "Publications");
        }
    }
}
