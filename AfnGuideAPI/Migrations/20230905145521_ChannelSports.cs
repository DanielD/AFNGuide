using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AfnGuideAPI.Migrations
{
    /// <inheritdoc />
    public partial class ChannelSports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "Channels");

            migrationBuilder.AddColumn<bool>(
                name: "IsSports",
                table: "Channels",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData("Channels", "Id", 1, "IsSports", true);
            migrationBuilder.UpdateData("Channels", "Id", 2, "IsSports", true);
            migrationBuilder.UpdateData("Channels", "Id", 4, "IsSports", true);
            migrationBuilder.UpdateData("Channels", "Id", 6, "IsSports", true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSports",
                table: "Channels");

            migrationBuilder.AddColumn<string>(
                name: "EndTime",
                table: "Channels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StartTime",
                table: "Channels",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
