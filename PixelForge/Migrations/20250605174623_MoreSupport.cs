using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PixelForge.Migrations
{
    /// <inheritdoc />
    public partial class MoreSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorId",
                table: "SupportMessages",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_SupportMessages_AuthorId",
                table: "SupportMessages",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_SupportMessages_AspNetUsers_AuthorId",
                table: "SupportMessages",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupportMessages_AspNetUsers_AuthorId",
                table: "SupportMessages");

            migrationBuilder.DropIndex(
                name: "IX_SupportMessages_AuthorId",
                table: "SupportMessages");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "SupportMessages");
        }
    }
}
