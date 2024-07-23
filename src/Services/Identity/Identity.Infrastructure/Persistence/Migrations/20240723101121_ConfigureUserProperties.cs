using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Identity.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureUserProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "Roles",
                keyColumn: "Id",
                keyValue: "8a65fd85-b18a-4d7b-b10b-ae1c54cc6d93");

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "Roles",
                keyColumn: "Id",
                keyValue: "dfddf5cc-41d4-43bd-8168-fbc0359c6f2f");

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "Roles",
                keyColumn: "Id",
                keyValue: "e0723160-46ec-42c9-ac13-c2ffe58fb2bc");

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "Roles",
                keyColumn: "Id",
                keyValue: "e831886b-4325-472f-9d13-3d9285f31e5e");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                schema: "Identity",
                table: "Users",
                type: "nvarchar(250)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AvatarUrl",
                schema: "Identity",
                table: "Users",
                type: "varchar(500)",
                nullable: true);

            migrationBuilder.InsertData(
                schema: "Identity",
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "068acf8e-c3aa-4e69-b03c-ff34b74c6368", null, "Administrator", "Administrator" },
                    { "b8b8ec4d-765f-419d-9b09-4271389acc87", null, "Author", "Author" },
                    { "c7e38fa8-8edc-4c1c-baad-86aaaacc6be3", null, "Subscriber ", "Subscriber " },
                    { "caf65413-38e9-4eb6-ab2a-cd4a2511609f", null, "Reader", "Reader" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "Roles",
                keyColumn: "Id",
                keyValue: "068acf8e-c3aa-4e69-b03c-ff34b74c6368");

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "Roles",
                keyColumn: "Id",
                keyValue: "b8b8ec4d-765f-419d-9b09-4271389acc87");

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "Roles",
                keyColumn: "Id",
                keyValue: "c7e38fa8-8edc-4c1c-baad-86aaaacc6be3");

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "Roles",
                keyColumn: "Id",
                keyValue: "caf65413-38e9-4eb6-ab2a-cd4a2511609f");

            migrationBuilder.DropColumn(
                name: "AvatarUrl",
                schema: "Identity",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                schema: "Identity",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldNullable: true);

            migrationBuilder.InsertData(
                schema: "Identity",
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "8a65fd85-b18a-4d7b-b10b-ae1c54cc6d93", null, "Author", "Author" },
                    { "dfddf5cc-41d4-43bd-8168-fbc0359c6f2f", null, "Reader", "Reader" },
                    { "e0723160-46ec-42c9-ac13-c2ffe58fb2bc", null, "Administrator", "Administrator" },
                    { "e831886b-4325-472f-9d13-3d9285f31e5e", null, "Subscriber ", "Subscriber " }
                });
        }
    }
}
