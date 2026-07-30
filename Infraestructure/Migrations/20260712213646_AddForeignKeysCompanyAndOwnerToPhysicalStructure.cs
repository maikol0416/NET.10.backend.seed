using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class AddForeignKeysCompanyAndOwnerToPhysicalStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Apartment_IdOwner",
                table: "Apartment",
                column: "IdOwner");

            migrationBuilder.AddForeignKey(
                name: "FK_Apartment_Owner_IdOwner",
                table: "Apartment",
                column: "IdOwner",
                principalTable: "Owner",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_PhysicalStructures_ManagementCompanies_CompanyId",
                table: "PhysicalStructures",
                column: "CompanyId",
                principalTable: "ManagementCompanies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Apartment_Owner_IdOwner",
                table: "Apartment");

            migrationBuilder.DropForeignKey(
                name: "FK_PhysicalStructures_ManagementCompanies_CompanyId",
                table: "PhysicalStructures");

            migrationBuilder.DropIndex(
                name: "IX_Apartment_IdOwner",
                table: "Apartment");
        }
    }
}
