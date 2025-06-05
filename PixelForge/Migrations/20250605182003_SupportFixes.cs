using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PixelForge.Migrations
{
    /// <inheritdoc />
    public partial class SupportFixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PublisherReply",
                table: "SupportMessages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReplyDate",
                table: "SupportMessages",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublisherReply",
                table: "SupportMessages");

            migrationBuilder.DropColumn(
                name: "ReplyDate",
                table: "SupportMessages");
        }
    }
}
