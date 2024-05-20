using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Identity.Api.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Create_Permissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "Roles",
                keyColumn: "Id",
                keyValue: "9d91e40c-00ba-4abb-9dc0-5e793b1932d9");

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "Roles",
                keyColumn: "Id",
                keyValue: "e60ba58c-b1a9-4088-9074-b6feed3741df");

            migrationBuilder.CreateTable(
                name: "Permissions",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Function = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    Command = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    RoleId = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Permissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Identity",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "Identity",
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "d57bd20b-7ba5-4a47-a65e-bf50860e8ee9", null, "Member", "MEMBER" },
                    { "f9a93c73-eb75-4037-9e40-475e8d18cd06", null, "Administrator", "ADMINISTRATOR" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_RoleId_Function_Command",
                schema: "Identity",
                table: "Permissions",
                columns: new[] { "RoleId", "Function", "Command" },
                unique: true,
                filter: "[Function] IS NOT NULL AND [Command] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Permissions",
                schema: "Identity");

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "Roles",
                keyColumn: "Id",
                keyValue: "d57bd20b-7ba5-4a47-a65e-bf50860e8ee9");

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "Roles",
                keyColumn: "Id",
                keyValue: "f9a93c73-eb75-4037-9e40-475e8d18cd06");

            migrationBuilder.InsertData(
                schema: "Identity",
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "9d91e40c-00ba-4abb-9dc0-5e793b1932d9", null, "Administrator", "ADMINISTRATOR" },
                    { "e60ba58c-b1a9-4088-9074-b6feed3741df", null, "Member", "MEMBER" }
                });
        }
    }
}
