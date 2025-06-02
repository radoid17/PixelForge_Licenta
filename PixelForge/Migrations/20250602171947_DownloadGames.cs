using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PixelForge.Migrations
{
    /// <inheritdoc />
    public partial class DownloadGames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GameFilePath",
                table: "Games",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GameFilePath",
                table: "Games");
        }
    }
}
