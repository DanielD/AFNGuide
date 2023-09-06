using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AfnGuideAPI.Migrations
{
    /// <inheritdoc />
    public partial class SportsUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SportId",
                table: "SportsSchedules");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOnUTC",
                table: "SportsSchedules",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOnUTC",
                table: "SportsSchedules",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedOnUTC",
                table: "SportsSchedules");

            migrationBuilder.DropColumn(
                name: "ModifiedOnUTC",
                table: "SportsSchedules");

            migrationBuilder.AddColumn<int>(
                name: "SportId",
                table: "SportsSchedules",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
