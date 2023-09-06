using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AfnGuideAPI.Migrations
{
    /// <inheritdoc />
    public partial class SportsTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SportsCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOnUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOnUTC = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SportsCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SportsNetworks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOnUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOnUTC = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SportsNetworks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SportsSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AfnId = table.Column<int>(type: "int", nullable: false),
                    ChannelId = table.Column<int>(type: "int", nullable: false),
                    SportId = table.Column<int>(type: "int", nullable: false),
                    SportName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsTapeDelayed = table.Column<bool>(type: "bit", nullable: false),
                    IsLive = table.Column<bool>(type: "bit", nullable: false),
                    AirDateUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SportsNetworkId = table.Column<int>(type: "int", nullable: true),
                    SportsCategoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SportsSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SportsSchedules_Channels_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SportsSchedules_SportsCategories_SportsCategoryId",
                        column: x => x.SportsCategoryId,
                        principalTable: "SportsCategories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SportsSchedules_SportsNetworks_SportsNetworkId",
                        column: x => x.SportsNetworkId,
                        principalTable: "SportsNetworks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SportsSchedules_ChannelId",
                table: "SportsSchedules",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_SportsSchedules_SportsCategoryId",
                table: "SportsSchedules",
                column: "SportsCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SportsSchedules_SportsNetworkId",
                table: "SportsSchedules",
                column: "SportsNetworkId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SportsSchedules");

            migrationBuilder.DropTable(
                name: "SportsCategories");

            migrationBuilder.DropTable(
                name: "SportsNetworks");
        }
    }
}
