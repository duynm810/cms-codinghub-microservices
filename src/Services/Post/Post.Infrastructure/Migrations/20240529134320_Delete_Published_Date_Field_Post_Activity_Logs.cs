using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Post.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Delete_Published_Date_Field_Post_Activity_Logs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "published_date",
                table: "PostActivityLogs");

            migrationBuilder.AddColumn<DateTime>(
                name: "published_date",
                table: "Posts",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "published_date",
                table: "Posts");

            migrationBuilder.AddColumn<DateTime>(
                name: "published_date",
                table: "PostActivityLogs",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}
