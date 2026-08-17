using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPhysicalStructureAndApartmentToGuestPermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ApartmentId",
                table: "GuestPermission",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PhysicalStructureId",
                table: "GuestPermission",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_GuestPermission_PhysicalStructureId",
                table: "GuestPermission",
                column: "PhysicalStructureId");

            migrationBuilder.AddForeignKey(
                name: "FK_GuestPermission_PhysicalStructures_PhysicalStructureId",
                table: "GuestPermission",
                column: "PhysicalStructureId",
                principalTable: "PhysicalStructures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GuestPermission_PhysicalStructures_PhysicalStructureId",
                table: "GuestPermission");

            migrationBuilder.DropIndex(
                name: "IX_GuestPermission_PhysicalStructureId",
                table: "GuestPermission");

            migrationBuilder.DropColumn(
                name: "ApartmentId",
                table: "GuestPermission");

            migrationBuilder.DropColumn(
                name: "PhysicalStructureId",
                table: "GuestPermission");
        }
    }
}
