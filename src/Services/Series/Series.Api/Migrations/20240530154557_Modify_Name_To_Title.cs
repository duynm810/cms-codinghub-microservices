using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Series.Api.Migrations
{
    /// <inheritdoc />
    public partial class Modify_Name_To_Title : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Series",
                newName: "Title");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Series",
                newName: "Name");
        }
    }
}
