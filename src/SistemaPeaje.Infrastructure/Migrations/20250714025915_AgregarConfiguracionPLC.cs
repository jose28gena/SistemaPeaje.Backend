using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaPeaje.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarConfiguracionPLC : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlcConfiguraciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Ip = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Puerto = table.Column<int>(type: "int", nullable: false),
                    UnitId = table.Column<byte>(type: "tinyint", nullable: false),
                    DireccionInicial = table.Column<int>(type: "int", nullable: false),
                    CantidadCoils = table.Column<int>(type: "int", nullable: false),
                    IntervaloMonitoreo = table.Column<int>(type: "int", nullable: false),
                    HabilitarLoggingPeriodico = table.Column<bool>(type: "bit", nullable: false),
                    EstacionId = table.Column<int>(type: "int", nullable: false),
                    CarrilId = table.Column<int>(type: "int", nullable: false),
                    EstaConectado = table.Column<bool>(type: "bit", nullable: false),
                    UltimaConexion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlcConfiguraciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlcConfiguraciones_Carriles_CarrilId",
                        column: x => x.CarrilId,
                        principalTable: "Carriles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlcConfiguraciones_Estaciones_EstacionId",
                        column: x => x.EstacionId,
                        principalTable: "Estaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlcCoilConfiguraciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlcConfiguracionId = table.Column<int>(type: "int", nullable: false),
                    Indice = table.Column<int>(type: "int", nullable: false),
                    Direccion = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TipoEvento = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GenerarEvento = table.Column<bool>(type: "bit", nullable: false),
                    EsAlarma = table.Column<bool>(type: "bit", nullable: false),
                    EstadoActual = table.Column<bool>(type: "bit", nullable: false),
                    UltimaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AccionEspecial = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlcCoilConfiguraciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlcCoilConfiguraciones_PlcConfiguraciones_PlcConfiguracionId",
                        column: x => x.PlcConfiguracionId,
                        principalTable: "PlcConfiguraciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlcCoilConfiguracion_PlcId_Indice",
                table: "PlcCoilConfiguraciones",
                columns: new[] { "PlcConfiguracionId", "Indice" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlcConfiguracion_Ip_Puerto",
                table: "PlcConfiguraciones",
                columns: new[] { "Ip", "Puerto" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlcConfiguraciones_CarrilId",
                table: "PlcConfiguraciones",
                column: "CarrilId");

            migrationBuilder.CreateIndex(
                name: "IX_PlcConfiguraciones_EstacionId",
                table: "PlcConfiguraciones",
                column: "EstacionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlcCoilConfiguraciones");

            migrationBuilder.DropTable(
                name: "PlcConfiguraciones");
        }
    }
}
