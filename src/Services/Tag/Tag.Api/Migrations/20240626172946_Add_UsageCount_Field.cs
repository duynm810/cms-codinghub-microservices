using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tag.Api.Migrations
{
    /// <inheritdoc />
    public partial class Add_UsageCount_Field : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UsageCount",
                table: "Tags",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsageCount",
                table: "Tags");
        }
    }
}
