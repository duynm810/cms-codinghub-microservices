﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Category.Api.Migrations
{
    /// <inheritdoc />
    public partial class Add_IsStaticPage_Field : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsStaticPage",
                table: "Categories",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsStaticPage",
                table: "Categories");
        }
    }
}
