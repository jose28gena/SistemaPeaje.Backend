using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaPeaje.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCamposCuadreTurno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CantidadExentos",
                table: "Turnos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CarrilId",
                table: "Turnos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "EfectivoContado",
                table: "Turnos",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VentasEfectivo",
                table: "Turnos",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VentasPrepago",
                table: "Turnos",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Turnos_CarrilId",
                table: "Turnos",
                column: "CarrilId");

            migrationBuilder.AddForeignKey(
                name: "FK_Turnos_Carriles_CarrilId",
                table: "Turnos",
                column: "CarrilId",
                principalTable: "Carriles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Turnos_Carriles_CarrilId",
                table: "Turnos");

            migrationBuilder.DropIndex(
                name: "IX_Turnos_CarrilId",
                table: "Turnos");

            migrationBuilder.DropColumn(
                name: "CantidadExentos",
                table: "Turnos");

            migrationBuilder.DropColumn(
                name: "CarrilId",
                table: "Turnos");

            migrationBuilder.DropColumn(
                name: "EfectivoContado",
                table: "Turnos");

            migrationBuilder.DropColumn(
                name: "VentasEfectivo",
                table: "Turnos");

            migrationBuilder.DropColumn(
                name: "VentasPrepago",
                table: "Turnos");
        }
    }
}
