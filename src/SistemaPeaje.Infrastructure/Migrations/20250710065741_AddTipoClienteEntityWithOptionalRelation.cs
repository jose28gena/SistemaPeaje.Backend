using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaPeaje.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTipoClienteEntityWithOptionalRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TipoClienteId",
                table: "Clientes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TiposCliente",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EstaExentoPago = table.Column<bool>(type: "bit", nullable: false),
                    DescuentoPorcentaje = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    RequiereValidacionDocumento = table.Column<bool>(type: "bit", nullable: false),
                    DocumentosRequeridos = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    VigenciaMeses = table.Column<int>(type: "int", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposCliente", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_TipoClienteId",
                table: "Clientes",
                column: "TipoClienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_TiposCliente_TipoClienteId",
                table: "Clientes",
                column: "TipoClienteId",
                principalTable: "TiposCliente",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_TiposCliente_TipoClienteId",
                table: "Clientes");

            migrationBuilder.DropTable(
                name: "TiposCliente");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_TipoClienteId",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "TipoClienteId",
                table: "Clientes");
        }
    }
}
