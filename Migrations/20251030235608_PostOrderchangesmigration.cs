using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TiendaProyecto.Migrations
{
    /// <inheritdoc />
    public partial class PostOrderchangesmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChangeReason",
                table: "Orders",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChangedByAdminId",
                table: "Orders",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Orders",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "StatusChangedAt",
                table: "Orders",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChangeReason",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ChangedByAdminId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "StatusChangedAt",
                table: "Orders");
        }
    }
}
