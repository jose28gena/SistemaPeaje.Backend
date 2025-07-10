using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaPeaje.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddComandoPlcEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ComandosPlc",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoComando = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IpDestino = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Puerto = table.Column<int>(type: "int", nullable: false),
                    UnitId = table.Column<byte>(type: "tinyint", nullable: false),
                    CoilAddress = table.Column<int>(type: "int", nullable: false),
                    ValorEnviado = table.Column<bool>(type: "bit", nullable: false),
                    ComandoExitoso = table.Column<bool>(type: "bit", nullable: false),
                    MensajeError = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CarrilId = table.Column<int>(type: "int", nullable: true),
                    UsuarioId = table.Column<int>(type: "int", nullable: true),
                    EmpleadoId = table.Column<int>(type: "int", nullable: true),
                    FechaEjecucion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TiempoRespuestaMs = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComandosPlc", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComandosPlc_Carriles_CarrilId",
                        column: x => x.CarrilId,
                        principalTable: "Carriles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComandosPlc_Empleados_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "Empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComandosPlc_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComandosPlc_CarrilId",
                table: "ComandosPlc",
                column: "CarrilId");

            migrationBuilder.CreateIndex(
                name: "IX_ComandosPlc_EmpleadoId",
                table: "ComandosPlc",
                column: "EmpleadoId");

            migrationBuilder.CreateIndex(
                name: "IX_ComandosPlc_UsuarioId",
                table: "ComandosPlc",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComandosPlc");
        }
    }
}
