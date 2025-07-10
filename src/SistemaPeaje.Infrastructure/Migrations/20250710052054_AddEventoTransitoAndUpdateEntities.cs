using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaPeaje.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEventoTransitoAndUpdateEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Categoria",
                table: "TiposVehiculo",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "EsActivo",
                table: "TiposVehiculo",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EsActivo",
                table: "TiposPago",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "LimiteCredito",
                table: "TiposPago",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequiereAutorizacion",
                table: "TiposPago",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "EventosTransito",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EstacionId = table.Column<int>(type: "int", nullable: false),
                    CarrilId = table.Column<int>(type: "int", nullable: false),
                    FechaEvento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoEvento = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DatosAdicionales = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PlacaVehiculo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TipoVehiculoId = table.Column<int>(type: "int", nullable: true),
                    CodigoRfid = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Procesado = table.Column<bool>(type: "bit", nullable: false),
                    TransaccionId = table.Column<int>(type: "int", nullable: true),
                    RutaImagen = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EstadoEvento = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    VelocidadVehiculo = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    SensorId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventosTransito", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventosTransito_Carriles_CarrilId",
                        column: x => x.CarrilId,
                        principalTable: "Carriles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventosTransito_Estaciones_EstacionId",
                        column: x => x.EstacionId,
                        principalTable: "Estaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventosTransito_TiposVehiculo_TipoVehiculoId",
                        column: x => x.TipoVehiculoId,
                        principalTable: "TiposVehiculo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EventosTransito_Transacciones_TransaccionId",
                        column: x => x.TransaccionId,
                        principalTable: "Transacciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventosTransito_CarrilId",
                table: "EventosTransito",
                column: "CarrilId");

            migrationBuilder.CreateIndex(
                name: "IX_EventosTransito_EstacionId",
                table: "EventosTransito",
                column: "EstacionId");

            migrationBuilder.CreateIndex(
                name: "IX_EventosTransito_TipoVehiculoId",
                table: "EventosTransito",
                column: "TipoVehiculoId");

            migrationBuilder.CreateIndex(
                name: "IX_EventosTransito_TransaccionId",
                table: "EventosTransito",
                column: "TransaccionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventosTransito");

            migrationBuilder.DropColumn(
                name: "Categoria",
                table: "TiposVehiculo");

            migrationBuilder.DropColumn(
                name: "EsActivo",
                table: "TiposVehiculo");

            migrationBuilder.DropColumn(
                name: "EsActivo",
                table: "TiposPago");

            migrationBuilder.DropColumn(
                name: "LimiteCredito",
                table: "TiposPago");

            migrationBuilder.DropColumn(
                name: "RequiereAutorizacion",
                table: "TiposPago");
        }
    }
}
