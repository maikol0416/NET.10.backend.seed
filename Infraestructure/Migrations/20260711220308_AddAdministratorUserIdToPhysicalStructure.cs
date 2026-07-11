using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAdministratorUserIdToPhysicalStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdministratorUserId",
                table: "PhysicalStructures",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhysicalStructures_AdministratorUserId",
                table: "PhysicalStructures",
                column: "AdministratorUserId");

            // FK física hacia AspNetUsers.Id (BC Identity). No se puede declarar vía Fluent
            // API/HasForeignKey porque AspNetUsers pertenece a IdentityAppDbContext, un
            // DbContext y modelo EF Core distintos de EntityDbContext (ver EntityDBSets) —
            // ambos comparten la misma base de datos física, así que la constraint se agrega
            // aquí a mano para mantener la integridad referencial a nivel de base de datos.
            // SetNull: si se elimina el usuario administrador, la estructura queda sin
            // administrador asignado en lugar de bloquear o arrastrar el borrado.
            migrationBuilder.AddForeignKey(
                name: "FK_PhysicalStructures_AspNetUsers_AdministratorUserId",
                table: "PhysicalStructures",
                column: "AdministratorUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhysicalStructures_AspNetUsers_AdministratorUserId",
                table: "PhysicalStructures");

            migrationBuilder.DropIndex(
                name: "IX_PhysicalStructures_AdministratorUserId",
                table: "PhysicalStructures");

            migrationBuilder.DropColumn(
                name: "AdministratorUserId",
                table: "PhysicalStructures");
        }
    }
}
