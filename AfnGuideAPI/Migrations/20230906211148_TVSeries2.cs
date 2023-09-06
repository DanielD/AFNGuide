using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AfnGuideAPI.Migrations
{
    /// <inheritdoc />
    public partial class TVSeries2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSplit",
                table: "TVSeries",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSplit",
                table: "TVSeries");
        }
    }
}
