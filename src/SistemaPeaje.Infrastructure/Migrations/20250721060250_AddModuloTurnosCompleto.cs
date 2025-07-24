using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaPeaje.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddModuloTurnosCompleto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FinDescanso",
                table: "Turnos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "HoraFinReal",
                table: "Turnos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "HoraInicioReal",
                table: "Turnos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "InicioDescanso",
                table: "Turnos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinutosDescanso",
                table: "Turnos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinutosHorasExtras",
                table: "Turnos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "MontoHorasExtras",
                table: "Turnos",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotivoCierre",
                table: "Turnos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NotasAdministrativas",
                table: "Turnos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Observaciones",
                table: "Turnos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TieneHorasExtras",
                table: "Turnos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalRecaudado",
                table: "Turnos",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TotalTransacciones",
                table: "Turnos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalVehiculos",
                table: "Turnos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TurnoAsignacionId",
                table: "Turnos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TurnoTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HoraInicio = table.Column<TimeSpan>(type: "time", nullable: false),
                    HoraFin = table.Column<TimeSpan>(type: "time", nullable: false),
                    DuracionMinutos = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EsActivo = table.Column<bool>(type: "bit", nullable: false),
                    PermiteHorasExtras = table.Column<bool>(type: "bit", nullable: false),
                    MaximoHorasExtras = table.Column<int>(type: "int", nullable: false),
                    FactorHoraExtra = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MinutosDescanso = table.Column<int>(type: "int", nullable: false),
                    HoraDescansoInicio = table.Column<TimeSpan>(type: "time", nullable: true),
                    Lunes = table.Column<bool>(type: "bit", nullable: false),
                    Martes = table.Column<bool>(type: "bit", nullable: false),
                    Miercoles = table.Column<bool>(type: "bit", nullable: false),
                    Jueves = table.Column<bool>(type: "bit", nullable: false),
                    Viernes = table.Column<bool>(type: "bit", nullable: false),
                    Sabado = table.Column<bool>(type: "bit", nullable: false),
                    Domingo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TurnoTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TurnoAsignaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpleadoId = table.Column<int>(type: "int", nullable: false),
                    EstacionId = table.Column<int>(type: "int", nullable: false),
                    TurnoTemplateId = table.Column<int>(type: "int", nullable: true),
                    FechaTurno = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoraInicioPrograma = table.Column<TimeSpan>(type: "time", nullable: false),
                    HoraFinPrograma = table.Column<TimeSpan>(type: "time", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaConfirmacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConfirmadoPorEmpleado = table.Column<bool>(type: "bit", nullable: false),
                    MotivoRechazo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmpleadoSustitutoId = table.Column<int>(type: "int", nullable: true),
                    MotivoSustitucion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaSustitucion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notas = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequiereSupervisor = table.Column<bool>(type: "bit", nullable: false),
                    MontoInicialCajaAsignado = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    InstruccionesEspeciales = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TurnoAsignaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TurnoAsignaciones_Empleados_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "Empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TurnoAsignaciones_Empleados_EmpleadoSustitutoId",
                        column: x => x.EmpleadoSustitutoId,
                        principalTable: "Empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TurnoAsignaciones_Estaciones_EstacionId",
                        column: x => x.EstacionId,
                        principalTable: "Estaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TurnoAsignaciones_TurnoTemplates_TurnoTemplateId",
                        column: x => x.TurnoTemplateId,
                        principalTable: "TurnoTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RegistrosTiempo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpleadoId = table.Column<int>(type: "int", nullable: false),
                    TurnoAsignacionId = table.Column<int>(type: "int", nullable: true),
                    TurnoId = table.Column<int>(type: "int", nullable: true),
                    FechaHora = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoRegistro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MetodoRegistro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstacionId = table.Column<int>(type: "int", nullable: true),
                    UbicacionGPS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DireccionIP = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EsValido = table.Column<bool>(type: "bit", nullable: false),
                    MotivoInvalidacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AutorizadoPorId = table.Column<int>(type: "int", nullable: true),
                    FechaAutorizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EsHoraExtra = table.Column<bool>(type: "bit", nullable: false),
                    EsAtraso = table.Column<bool>(type: "bit", nullable: false),
                    MinutosAtraso = table.Column<int>(type: "int", nullable: false),
                    ArchivoAdjunto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosTiempo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrosTiempo_Empleados_AutorizadoPorId",
                        column: x => x.AutorizadoPorId,
                        principalTable: "Empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegistrosTiempo_Empleados_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "Empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegistrosTiempo_Estaciones_EstacionId",
                        column: x => x.EstacionId,
                        principalTable: "Estaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegistrosTiempo_TurnoAsignaciones_TurnoAsignacionId",
                        column: x => x.TurnoAsignacionId,
                        principalTable: "TurnoAsignaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegistrosTiempo_Turnos_TurnoId",
                        column: x => x.TurnoId,
                        principalTable: "Turnos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TurnoEventos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TurnoId = table.Column<int>(type: "int", nullable: true),
                    TurnoAsignacionId = table.Column<int>(type: "int", nullable: true),
                    EmpleadoId = table.Column<int>(type: "int", nullable: false),
                    EstacionId = table.Column<int>(type: "int", nullable: true),
                    FechaHoraEvento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoEvento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prioridad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaResolucion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DuracionMinutos = table.Column<int>(type: "int", nullable: true),
                    AfectaOperacion = table.Column<bool>(type: "bit", nullable: false),
                    ImpactoOperacional = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MontoAfectado = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ReportadoPorId = table.Column<int>(type: "int", nullable: true),
                    AsignadoAId = table.Column<int>(type: "int", nullable: true),
                    ResueltoPorId = table.Column<int>(type: "int", nullable: true),
                    AccionesTomadas = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SolucionAplicada = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MedidasPreventivas = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArchivosAdjuntos = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequiereNotificacion = table.Column<bool>(type: "bit", nullable: false),
                    PersonasNotificadas = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TurnoEventos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TurnoEventos_Empleados_AsignadoAId",
                        column: x => x.AsignadoAId,
                        principalTable: "Empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TurnoEventos_Empleados_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "Empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TurnoEventos_Empleados_ReportadoPorId",
                        column: x => x.ReportadoPorId,
                        principalTable: "Empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TurnoEventos_Empleados_ResueltoPorId",
                        column: x => x.ResueltoPorId,
                        principalTable: "Empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TurnoEventos_Estaciones_EstacionId",
                        column: x => x.EstacionId,
                        principalTable: "Estaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TurnoEventos_TurnoAsignaciones_TurnoAsignacionId",
                        column: x => x.TurnoAsignacionId,
                        principalTable: "TurnoAsignaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TurnoEventos_Turnos_TurnoId",
                        column: x => x.TurnoId,
                        principalTable: "Turnos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Turnos_TurnoAsignacionId",
                table: "Turnos",
                column: "TurnoAsignacionId",
                unique: true,
                filter: "[TurnoAsignacionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosTiempo_AutorizadoPorId",
                table: "RegistrosTiempo",
                column: "AutorizadoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosTiempo_EmpleadoId",
                table: "RegistrosTiempo",
                column: "EmpleadoId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosTiempo_EstacionId",
                table: "RegistrosTiempo",
                column: "EstacionId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosTiempo_TurnoAsignacionId",
                table: "RegistrosTiempo",
                column: "TurnoAsignacionId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosTiempo_TurnoId",
                table: "RegistrosTiempo",
                column: "TurnoId");

            migrationBuilder.CreateIndex(
                name: "IX_TurnoAsignaciones_EmpleadoId",
                table: "TurnoAsignaciones",
                column: "EmpleadoId");

            migrationBuilder.CreateIndex(
                name: "IX_TurnoAsignaciones_EmpleadoSustitutoId",
                table: "TurnoAsignaciones",
                column: "EmpleadoSustitutoId");

            migrationBuilder.CreateIndex(
                name: "IX_TurnoAsignaciones_EstacionId",
                table: "TurnoAsignaciones",
                column: "EstacionId");

            migrationBuilder.CreateIndex(
                name: "IX_TurnoAsignaciones_TurnoTemplateId",
                table: "TurnoAsignaciones",
                column: "TurnoTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_TurnoEventos_AsignadoAId",
                table: "TurnoEventos",
                column: "AsignadoAId");

            migrationBuilder.CreateIndex(
                name: "IX_TurnoEventos_EmpleadoId",
                table: "TurnoEventos",
                column: "EmpleadoId");

            migrationBuilder.CreateIndex(
                name: "IX_TurnoEventos_EstacionId",
                table: "TurnoEventos",
                column: "EstacionId");

            migrationBuilder.CreateIndex(
                name: "IX_TurnoEventos_ReportadoPorId",
                table: "TurnoEventos",
                column: "ReportadoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_TurnoEventos_ResueltoPorId",
                table: "TurnoEventos",
                column: "ResueltoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_TurnoEventos_TurnoAsignacionId",
                table: "TurnoEventos",
                column: "TurnoAsignacionId");

            migrationBuilder.CreateIndex(
                name: "IX_TurnoEventos_TurnoId",
                table: "TurnoEventos",
                column: "TurnoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Turnos_TurnoAsignaciones_TurnoAsignacionId",
                table: "Turnos",
                column: "TurnoAsignacionId",
                principalTable: "TurnoAsignaciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Turnos_TurnoAsignaciones_TurnoAsignacionId",
                table: "Turnos");

            migrationBuilder.DropTable(
                name: "RegistrosTiempo");

            migrationBuilder.DropTable(
                name: "TurnoEventos");

            migrationBuilder.DropTable(
                name: "TurnoAsignaciones");

            migrationBuilder.DropTable(
                name: "TurnoTemplates");

            migrationBuilder.DropIndex(
                name: "IX_Turnos_TurnoAsignacionId",
                table: "Turnos");

            migrationBuilder.DropColumn(
                name: "FinDescanso",
                table: "Turnos");

            migrationBuilder.DropColumn(
                name: "HoraFinReal",
                table: "Turnos");

            migrationBuilder.DropColumn(
                name: "HoraInicioReal",
                table: "Turnos");

            migrationBuilder.DropColumn(
                name: "InicioDescanso",
                table: "Turnos");

            migrationBuilder.DropColumn(
                name: "MinutosDescanso",
                table: "Turnos");

            migrationBuilder.DropColumn(
                name: "MinutosHorasExtras",
                table: "Turnos");

            migrationBuilder.DropColumn(
                name: "MontoHorasExtras",
                table: "Turnos");

            migrationBuilder.DropColumn(
                name: "MotivoCierre",
                table: "Turnos");

            migrationBuilder.DropColumn(
                name: "NotasAdministrativas",
                table: "Turnos");

            migrationBuilder.DropColumn(
                name: "Observaciones",
                table: "Turnos");

            migrationBuilder.DropColumn(
                name: "TieneHorasExtras",
                table: "Turnos");

            migrationBuilder.DropColumn(
                name: "TotalRecaudado",
                table: "Turnos");

            migrationBuilder.DropColumn(
                name: "TotalTransacciones",
                table: "Turnos");

            migrationBuilder.DropColumn(
                name: "TotalVehiculos",
                table: "Turnos");

            migrationBuilder.DropColumn(
                name: "TurnoAsignacionId",
                table: "Turnos");
        }
    }
}
