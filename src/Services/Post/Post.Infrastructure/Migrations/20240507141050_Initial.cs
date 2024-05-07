using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Post.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Slug = table.Column<string>(type: "varchar(250)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Thumbnail = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SeoDescription = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Source = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ViewCount = table.Column<int>(type: "int", nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    RoyaltyAmount = table.Column<double>(type: "float", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CategoryId = table.Column<long>(type: "bigint", nullable: false),
                    AuthorUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaidDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_Slug",
                table: "Posts",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Posts");
        }
    }
}
