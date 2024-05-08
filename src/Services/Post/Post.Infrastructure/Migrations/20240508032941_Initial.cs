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
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    slug = table.Column<string>(type: "varchar(250)", nullable: false),
                    content = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    thumbnail = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    seo_description = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    source = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    tags = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    view_count = table.Column<int>(type: "integer", nullable: false),
                    is_paid = table.Column<bool>(type: "boolean", nullable: false),
                    royalty_amount = table.Column<double>(type: "double precision", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    category_id = table.Column<long>(type: "bigint", nullable: false),
                    author_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    paid_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_modified_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_posts", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_posts_slug",
                table: "Posts",
                column: "slug",
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
