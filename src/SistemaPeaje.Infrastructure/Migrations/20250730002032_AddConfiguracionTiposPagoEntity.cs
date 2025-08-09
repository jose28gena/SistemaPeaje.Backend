using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaPeaje.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddConfiguracionTiposPagoEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfiguracionTipoPago",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoPagoId = table.Column<int>(type: "int", nullable: false),
                    Moneda = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SimboloMoneda = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    LimiteDiario = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LimiteTransaccion = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ComisionPorcentaje = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ComisionFija = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EstaActivo = table.Column<bool>(type: "bit", nullable: false),
                    RequiereValidacionAdicional = table.Column<bool>(type: "bit", nullable: false),
                    TiempoEsperaSegundos = table.Column<int>(type: "int", nullable: false),
                    DescuentoPorDefecto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PermiteTransaccionesParcialeS = table.Column<bool>(type: "bit", nullable: false),
                    ConfiguracionEspecifica = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionTipoPago", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfiguracionTipoPago_TiposPago_TipoPagoId",
                        column: x => x.TipoPagoId,
                        principalTable: "TiposPago",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracionTipoPago_TipoPagoId",
                table: "ConfiguracionTipoPago",
                column: "TipoPagoId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfiguracionTipoPago");
        }
    }
}
