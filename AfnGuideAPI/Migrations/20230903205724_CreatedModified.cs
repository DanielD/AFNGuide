using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AfnGuideAPI.Migrations
{
    /// <inheritdoc />
    public partial class CreatedModified : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOnUTC",
                table: "TimeZones",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOnUTC",
                table: "TimeZones",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOnUTC",
                table: "Schedules",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOnUTC",
                table: "Promos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOnUTC",
                table: "Channels",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOnUTC",
                table: "Channels",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOnUTC",
                table: "Bulletins",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedOnUTC",
                table: "TimeZones");

            migrationBuilder.DropColumn(
                name: "ModifiedOnUTC",
                table: "TimeZones");

            migrationBuilder.DropColumn(
                name: "ModifiedOnUTC",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "ModifiedOnUTC",
                table: "Promos");

            migrationBuilder.DropColumn(
                name: "CreatedOnUTC",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "ModifiedOnUTC",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "ModifiedOnUTC",
                table: "Bulletins");
        }
    }
}
