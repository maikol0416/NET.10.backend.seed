using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class updateValueObjectsToEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "Towers");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "DocumentSignature");

            migrationBuilder.RenameColumn(
                name: "Created",
                table: "CommonArea",
                newName: "CreatedAt");

            // --- CORRECCIÓN EN EL MÉTODO UP ---
            // 1. Eliminamos la clave primaria actual
            migrationBuilder.DropPrimaryKey(
                name: "PK_CommonArea",
                table: "CommonArea");

            // 2. Eliminamos la columna Id (que era int identity)
            migrationBuilder.DropColumn(
                name: "Id",
                table: "CommonArea");

            // 3. Agregamos la nueva columna Id como Guid
            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "CommonArea",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWSEQUENTIALID()");

            // 4. Volvemos a crear la clave primaria
            migrationBuilder.AddPrimaryKey(
                name: "PK_CommonArea",
                table: "CommonArea",
                column: "Id");
            // ----------------------------------

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "CommonArea",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateAt",
                table: "CommonArea",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Detail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Neighborhood = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhysicalStructureId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Location_PhysicalStructures_PhysicalStructureId",
                        column: x => x.PhysicalStructureId,
                        principalTable: "PhysicalStructures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tower",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Number = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PhysicalStructureId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tower", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tower_PhysicalStructures_PhysicalStructureId",
                        column: x => x.PhysicalStructureId,
                        principalTable: "PhysicalStructures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Location_PhysicalStructureId",
                table: "Location",
                column: "PhysicalStructureId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tower_PhysicalStructureId",
                table: "Tower",
                column: "PhysicalStructureId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Location");

            migrationBuilder.DropTable(
                name: "Tower");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "CommonArea");

            migrationBuilder.DropColumn(
                name: "UpdateAt",
                table: "CommonArea");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "CommonArea",
                newName: "Created");

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "DocumentSignature",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            // --- CORRECCIÓN EN EL MÉTODO DOWN ---
            // 1. Eliminamos la clave primaria actual (que es Guid)
            migrationBuilder.DropPrimaryKey(
                name: "PK_CommonArea",
                table: "CommonArea");

            // 2. Eliminamos la columna Id (Guid)
            migrationBuilder.DropColumn(
                name: "Id",
                table: "CommonArea");

            // 3. Volvemos a agregar la columna Id como int autoincremental
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "CommonArea",
                type: "int",
                nullable: false)
                .Annotation("SqlServer:Identity", "1, 1");

            // 4. Restauramos la clave primaria
            migrationBuilder.AddPrimaryKey(
                name: "PK_CommonArea",
                table: "CommonArea",
                column: "Id");
            // ------------------------------------

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Detail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Neighborhood = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Number = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PhysicalStructureId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Locations_PhysicalStructures_PhysicalStructureId",
                        column: x => x.PhysicalStructureId,
                        principalTable: "PhysicalStructures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Towers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Number = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PhysicalStructureId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Towers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Towers_PhysicalStructures_PhysicalStructureId",
                        column: x => x.PhysicalStructureId,
                        principalTable: "PhysicalStructures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Locations_PhysicalStructureId",
                table: "Locations",
                column: "PhysicalStructureId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Towers_PhysicalStructureId",
                table: "Towers",
                column: "PhysicalStructureId");
        }
    }
}