using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AfnGuideAPI.Migrations
{
    /// <inheritdoc />
    public partial class TVSeries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SportsSchedules_Channels_ChannelId",
                table: "SportsSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_SportsSchedules_SportsCategories_SportsCategoryId",
                table: "SportsSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_SportsSchedules_SportsNetworks_SportsNetworkId",
                table: "SportsSchedules");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_SportsSchedules_AfnId",
                table: "SportsSchedules",
                column: "AfnId");

            migrationBuilder.CreateTable(
                name: "TVSeries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChannelId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Season = table.Column<int>(type: "int", nullable: true),
                    PremiereType = table.Column<int>(type: "int", nullable: true),
                    CreatedOnUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOnUTC = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TVSeries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TVSeries_Channels_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TVSeries_ChannelId",
                table: "TVSeries",
                column: "ChannelId");

            migrationBuilder.AddForeignKey(
                name: "FK_SportsSchedules_Channels_ChannelId",
                table: "SportsSchedules",
                column: "ChannelId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SportsSchedules_SportsCategories_SportsCategoryId",
                table: "SportsSchedules",
                column: "SportsCategoryId",
                principalTable: "SportsCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SportsSchedules_SportsNetworks_SportsNetworkId",
                table: "SportsSchedules",
                column: "SportsNetworkId",
                principalTable: "SportsNetworks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SportsSchedules_Channels_ChannelId",
                table: "SportsSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_SportsSchedules_SportsCategories_SportsCategoryId",
                table: "SportsSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_SportsSchedules_SportsNetworks_SportsNetworkId",
                table: "SportsSchedules");

            migrationBuilder.DropTable(
                name: "TVSeries");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_SportsSchedules_AfnId",
                table: "SportsSchedules");

            migrationBuilder.AddForeignKey(
                name: "FK_SportsSchedules_Channels_ChannelId",
                table: "SportsSchedules",
                column: "ChannelId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SportsSchedules_SportsCategories_SportsCategoryId",
                table: "SportsSchedules",
                column: "SportsCategoryId",
                principalTable: "SportsCategories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SportsSchedules_SportsNetworks_SportsNetworkId",
                table: "SportsSchedules",
                column: "SportsNetworkId",
                principalTable: "SportsNetworks",
                principalColumn: "Id");
        }
    }
}
