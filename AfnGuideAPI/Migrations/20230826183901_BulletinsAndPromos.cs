using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AfnGuideAPI.Migrations
{
    /// <inheritdoc />
    public partial class BulletinsAndPromos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_Schedules_AfnId",
                table: "Schedules",
                column: "AfnId");

            migrationBuilder.CreateTable(
                name: "Bulletins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOnUTC = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bulletins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Promos",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AfnId = table.Column<int>(type: "int", nullable: true),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Duration = table.Column<int>(type: "int", nullable: true),
                    IsPromoB = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOnUTC = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Promos_Schedules_AfnId",
                        column: x => x.AfnId,
                        principalTable: "Schedules",
                        principalColumn: "AfnId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Promos_AfnId",
                table: "Promos",
                column: "AfnId",
                unique: true,
                filter: "[AfnId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bulletins");

            migrationBuilder.DropTable(
                name: "Promos");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Schedules_AfnId",
                table: "Schedules");
        }
    }
}
