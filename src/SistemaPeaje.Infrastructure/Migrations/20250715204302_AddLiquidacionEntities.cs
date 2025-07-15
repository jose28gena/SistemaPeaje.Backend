using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaPeaje.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLiquidacionEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Liquidaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroLiquidacion = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TipoLiquidacion = table.Column<int>(type: "int", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaGeneracion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstacionId = table.Column<int>(type: "int", nullable: true),
                    EmpleadoId = table.Column<int>(type: "int", nullable: true),
                    TurnoId = table.Column<int>(type: "int", nullable: true),
                    MontoTotalTransacciones = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MontoTotalRecaudado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiferenciaCaja = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalTransacciones = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    RequiereAprobacion = table.Column<bool>(type: "bit", nullable: false),
                    FechaAprobacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AprobadoPorEmpleadoId = table.Column<int>(type: "int", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    NotasAprobacion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreadoPorEmpleadoId = table.Column<int>(type: "int", nullable: false),
                    FechaUltimaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Liquidaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Liquidaciones_Empleados_AprobadoPorEmpleadoId",
                        column: x => x.AprobadoPorEmpleadoId,
                        principalTable: "Empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Liquidaciones_Empleados_CreadoPorEmpleadoId",
                        column: x => x.CreadoPorEmpleadoId,
                        principalTable: "Empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Liquidaciones_Empleados_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "Empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Liquidaciones_Estaciones_EstacionId",
                        column: x => x.EstacionId,
                        principalTable: "Estaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Liquidaciones_Turnos_TurnoId",
                        column: x => x.TurnoId,
                        principalTable: "Turnos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LiquidacionDetalles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LiquidacionId = table.Column<int>(type: "int", nullable: false),
                    TransaccionId = table.Column<int>(type: "int", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TipoPago = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TipoVehiculo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FechaTransaccion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CarrilId = table.Column<int>(type: "int", nullable: false),
                    Validado = table.Column<bool>(type: "bit", nullable: false),
                    ObservacionesValidacion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiquidacionDetalles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LiquidacionDetalles_Liquidaciones_LiquidacionId",
                        column: x => x.LiquidacionId,
                        principalTable: "Liquidaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LiquidacionDetalles_Transacciones_TransaccionId",
                        column: x => x.TransaccionId,
                        principalTable: "Transacciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LiquidacionDiscrepancias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LiquidacionId = table.Column<int>(type: "int", nullable: false),
                    TipoDiscrepancia = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MontoDiscrepancia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Severidad = table.Column<int>(type: "int", nullable: false),
                    Resuelta = table.Column<bool>(type: "bit", nullable: false),
                    FechaResolucion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NotasResolucion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ResueltoPorEmpleadoId = table.Column<int>(type: "int", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiquidacionDiscrepancias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LiquidacionDiscrepancias_Empleados_ResueltoPorEmpleadoId",
                        column: x => x.ResueltoPorEmpleadoId,
                        principalTable: "Empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LiquidacionDiscrepancias_Liquidaciones_LiquidacionId",
                        column: x => x.LiquidacionId,
                        principalTable: "Liquidaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LiquidacionDetalle_LiquidacionId",
                table: "LiquidacionDetalles",
                column: "LiquidacionId");

            migrationBuilder.CreateIndex(
                name: "IX_LiquidacionDetalles_TransaccionId",
                table: "LiquidacionDetalles",
                column: "TransaccionId");

            migrationBuilder.CreateIndex(
                name: "IX_LiquidacionDiscrepancia_LiquidacionId",
                table: "LiquidacionDiscrepancias",
                column: "LiquidacionId");

            migrationBuilder.CreateIndex(
                name: "IX_LiquidacionDiscrepancia_Tipo_Severidad",
                table: "LiquidacionDiscrepancias",
                columns: new[] { "TipoDiscrepancia", "Severidad" });

            migrationBuilder.CreateIndex(
                name: "IX_LiquidacionDiscrepancias_ResueltoPorEmpleadoId",
                table: "LiquidacionDiscrepancias",
                column: "ResueltoPorEmpleadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Liquidacion_NumeroLiquidacion",
                table: "Liquidaciones",
                column: "NumeroLiquidacion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Liquidacion_Tipo_Fecha",
                table: "Liquidaciones",
                columns: new[] { "TipoLiquidacion", "FechaInicio", "FechaFin" });

            migrationBuilder.CreateIndex(
                name: "IX_Liquidaciones_AprobadoPorEmpleadoId",
                table: "Liquidaciones",
                column: "AprobadoPorEmpleadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Liquidaciones_CreadoPorEmpleadoId",
                table: "Liquidaciones",
                column: "CreadoPorEmpleadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Liquidaciones_EmpleadoId",
                table: "Liquidaciones",
                column: "EmpleadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Liquidaciones_EstacionId",
                table: "Liquidaciones",
                column: "EstacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Liquidaciones_TurnoId",
                table: "Liquidaciones",
                column: "TurnoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LiquidacionDetalles");

            migrationBuilder.DropTable(
                name: "LiquidacionDiscrepancias");

            migrationBuilder.DropTable(
                name: "Liquidaciones");
        }
    }
}
