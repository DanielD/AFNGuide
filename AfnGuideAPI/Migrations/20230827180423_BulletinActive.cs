using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AfnGuideAPI.Migrations
{
    /// <inheritdoc />
    public partial class BulletinActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Bulletins",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Bulletins");
        }
    }
}
