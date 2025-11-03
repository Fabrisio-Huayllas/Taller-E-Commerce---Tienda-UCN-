using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace TiendaProyecto.Migrations
{
    /// <inheritdoc />
    public partial class AddSlugAndFieldsToBrand : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Brands",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Brands",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Brands",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Brands",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Brands");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Brands");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Brands");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Brands");
        }
    }
}