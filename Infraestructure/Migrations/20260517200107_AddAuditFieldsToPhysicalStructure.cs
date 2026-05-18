using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditFieldsToPhysicalStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "created",
                table: "Locations",
                newName: "Created");

            migrationBuilder.RenameColumn(
                name: "created",
                table: "CommonArea",
                newName: "Created");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "PhysicalStructures",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "PhysicalStructures",
                type: "nvarchar(1)",
                maxLength: 1,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateAt",
                table: "PhysicalStructures",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "PhysicalStructures");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "PhysicalStructures");

            migrationBuilder.DropColumn(
                name: "UpdateAt",
                table: "PhysicalStructures");

            migrationBuilder.RenameColumn(
                name: "Created",
                table: "Locations",
                newName: "created");

            migrationBuilder.RenameColumn(
                name: "Created",
                table: "CommonArea",
                newName: "created");
        }
    }
}
