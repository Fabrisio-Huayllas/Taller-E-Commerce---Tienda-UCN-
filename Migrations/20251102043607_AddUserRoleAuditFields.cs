using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TiendaProyecto.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRoleAuditFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastRoleChangedAt",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastRoleChangedBy",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreviousRole",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastRoleChangedAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastRoleChangedBy",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PreviousRole",
                table: "AspNetUsers");
        }
    }
}
