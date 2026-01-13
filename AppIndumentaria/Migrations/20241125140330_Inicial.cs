using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppIndumentaria.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Capacitaciones",
                columns: table => new
                {
                    CapacitacionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Capacitaciones", x => x.CapacitacionID);
                });

            migrationBuilder.CreateTable(
                name: "Empleados",
                columns: table => new
                {
                    EmpleadoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Puesto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaIngreso = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empleados", x => x.EmpleadoID);
                });

            migrationBuilder.CreateTable(
                name: "Indumentarias",
                columns: table => new
                {
                    IndumentariaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Indumentarias", x => x.IndumentariaID);
                });

            migrationBuilder.CreateTable(
                name: "Talles",
                columns: table => new
                {
                    TalleID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Talles", x => x.TalleID);
                });

            migrationBuilder.CreateTable(
                name: "ParticipacionesCapacitaciones",
                columns: table => new
                {
                    ParticipacionCapacitacionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpleadoID = table.Column<int>(type: "int", nullable: false),
                    CapacitacionID = table.Column<int>(type: "int", nullable: false),
                    FechaParticipacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParticipacionesCapacitaciones", x => x.ParticipacionCapacitacionID);
                    table.ForeignKey(
                        name: "FK_ParticipacionesCapacitaciones_Capacitaciones_CapacitacionID",
                        column: x => x.CapacitacionID,
                        principalTable: "Capacitaciones",
                        principalColumn: "CapacitacionID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ParticipacionesCapacitaciones_Empleados_EmpleadoID",
                        column: x => x.EmpleadoID,
                        principalTable: "Empleados",
                        principalColumn: "EmpleadoID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntregasIndumentaria",
                columns: table => new
                {
                    EntregaIndumentariaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpleadoID = table.Column<int>(type: "int", nullable: false),
                    IndumentariaID = table.Column<int>(type: "int", nullable: false),
                    TalleID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FechaEntrega = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CantidadEntregada = table.Column<int>(type: "int", nullable: false),
                    Certificacion = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntregasIndumentaria", x => x.EntregaIndumentariaID);
                    table.ForeignKey(
                        name: "FK_EntregasIndumentaria_Empleados_EmpleadoID",
                        column: x => x.EmpleadoID,
                        principalTable: "Empleados",
                        principalColumn: "EmpleadoID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntregasIndumentaria_Indumentarias_IndumentariaID",
                        column: x => x.IndumentariaID,
                        principalTable: "Indumentarias",
                        principalColumn: "IndumentariaID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntregasIndumentaria_Talles_TalleID",
                        column: x => x.TalleID,
                        principalTable: "Talles",
                        principalColumn: "TalleID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TalleIndumentarias",
                columns: table => new
                {
                    TalleIndumentariaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IndumentariaID = table.Column<int>(type: "int", nullable: false),
                    TalleID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CantidadDisponible = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TalleIndumentarias", x => x.TalleIndumentariaID);
                    table.ForeignKey(
                        name: "FK_TalleIndumentarias_Indumentarias_IndumentariaID",
                        column: x => x.IndumentariaID,
                        principalTable: "Indumentarias",
                        principalColumn: "IndumentariaID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TalleIndumentarias_Talles_TalleID",
                        column: x => x.TalleID,
                        principalTable: "Talles",
                        principalColumn: "TalleID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EntregasIndumentaria_EmpleadoID",
                table: "EntregasIndumentaria",
                column: "EmpleadoID");

            migrationBuilder.CreateIndex(
                name: "IX_EntregasIndumentaria_IndumentariaID",
                table: "EntregasIndumentaria",
                column: "IndumentariaID");

            migrationBuilder.CreateIndex(
                name: "IX_EntregasIndumentaria_TalleID",
                table: "EntregasIndumentaria",
                column: "TalleID");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipacionesCapacitaciones_CapacitacionID",
                table: "ParticipacionesCapacitaciones",
                column: "CapacitacionID");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipacionesCapacitaciones_EmpleadoID",
                table: "ParticipacionesCapacitaciones",
                column: "EmpleadoID");

            migrationBuilder.CreateIndex(
                name: "IX_TalleIndumentarias_IndumentariaID",
                table: "TalleIndumentarias",
                column: "IndumentariaID");

            migrationBuilder.CreateIndex(
                name: "IX_TalleIndumentarias_TalleID",
                table: "TalleIndumentarias",
                column: "TalleID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EntregasIndumentaria");

            migrationBuilder.DropTable(
                name: "ParticipacionesCapacitaciones");

            migrationBuilder.DropTable(
                name: "TalleIndumentarias");

            migrationBuilder.DropTable(
                name: "Capacitaciones");

            migrationBuilder.DropTable(
                name: "Empleados");

            migrationBuilder.DropTable(
                name: "Indumentarias");

            migrationBuilder.DropTable(
                name: "Talles");
        }
    }
}
