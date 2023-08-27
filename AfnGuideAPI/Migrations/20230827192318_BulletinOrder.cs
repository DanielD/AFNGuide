using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AfnGuideAPI.Migrations
{
    /// <inheritdoc />
    public partial class BulletinOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Bulletins",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "Bulletins");
        }
    }
}
