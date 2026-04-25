using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaPeaje.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddClienteFlujosEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClienteVehiculoId",
                table: "TarjetasRFID",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EnListaNegra",
                table: "TarjetasRFID",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaBloqueo",
                table: "TarjetasRFID",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotivoBloqueo",
                table: "TarjetasRFID",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ObservacionesSeguridad",
                table: "TarjetasRFID",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoAsociacion",
                table: "TarjetasRFID",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UltimaActividad",
                table: "TarjetasRFID",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AlertaSaldoBajo",
                table: "Clientes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "DiasCredito",
                table: "Clientes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DocumentosValidados",
                table: "Clientes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DomicilioFiscal",
                table: "Clientes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailFacturacion",
                table: "Clientes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EstadoCliente",
                table: "Clientes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaAprobacion",
                table: "Clientes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCierre",
                table: "Clientes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaConsentimiento",
                table: "Clientes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCorteCredito",
                table: "Clientes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaSuspension",
                table: "Clientes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaValidacionDocumentos",
                table: "Clientes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaVencimientoDocumentos",
                table: "Clientes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModeloCuenta",
                table: "Clientes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "MontoAlertaSaldo",
                table: "Clientes",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotivoCierre",
                table: "Clientes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotivoSuspension",
                table: "Clientes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ObservacionesKYC",
                table: "Clientes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PeriodicidadFacturacion",
                table: "Clientes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreferenciaFacturacion",
                table: "Clientes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RFC",
                table: "Clientes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RazonSocial",
                table: "Clientes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegimenFiscal",
                table: "Clientes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SaldoMinimo",
                table: "Clientes",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SerieFacturacion",
                table: "Clientes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TopeCredito",
                table: "Clientes",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsoCFDI",
                table: "Clientes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioAprobacion",
                table: "Clientes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioCreacion",
                table: "Clientes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ClienteDocumento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    TipoDocumento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NombreArchivo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RutaArchivo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UrlArchivo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TipoMime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TamañoArchivo = table.Column<long>(type: "bigint", nullable: false),
                    FechaSubida = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EstadoValidacion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaValidacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioValidacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ObservacionesValidacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MotivoRechazo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EsObligatorio = table.Column<bool>(type: "bit", nullable: false),
                    VersionDocumento = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClienteDocumento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClienteDocumento_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClienteFactura",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    NumeroFactura = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Serie = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Folio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaFactura = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PeriodoInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PeriodoFin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Descuento = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IVA = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OtrosImpuestos = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TipoFactura = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MotivoFacturacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UUID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaTimbrado = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CadenaOriginal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SelloDigital = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RFCProveedorCertificacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumeroCertificadoSAT = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SelloSAT = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FormaPago = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MetodoPago = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsoCFDI = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaVencimiento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DiasCredito = table.Column<int>(type: "int", nullable: true),
                    RutaPDF = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RutaXML = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailEnvio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaEnvio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsuarioCreacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsuarioTimbrado = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClienteFactura", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClienteFactura_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClienteRecarga",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    TarjetaRFIDId = table.Column<int>(type: "int", nullable: true),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MedioPago = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Referencia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumeroAutorizacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaRecarga = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaConfirmacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaAplicacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioCreacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsuarioConfirmacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ComprobantePago = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequiereConciliacion = table.Column<bool>(type: "bit", nullable: false),
                    FechaConciliacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioConciliacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IPOrigen = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DispositivoOrigen = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MarcadaFraude = table.Column<bool>(type: "bit", nullable: false),
                    MotivoFraude = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClienteRecarga", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClienteRecarga_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClienteRecarga_TarjetasRFID_TarjetaRFIDId",
                        column: x => x.TarjetaRFIDId,
                        principalTable: "TarjetasRFID",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClienteVehiculo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    Placa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Marca = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Modelo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Año = table.Column<int>(type: "int", nullable: true),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VIN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumeroSerie = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaseVehiculo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubClaseVehiculo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Propietario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EsPrincipal = table.Column<bool>(type: "bit", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaVencimientoDocumentos = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClienteVehiculo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClienteVehiculo_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TicketSoporte",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    NumeroTicket = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prioridad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaApertura = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaAsignacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaResolucion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaCierre = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioCreacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsuarioAsignado = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsuarioResolucion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CanalReporte = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TarjetaRFIDAfectada = table.Column<int>(type: "int", nullable: true),
                    TransaccionAfectada = table.Column<int>(type: "int", nullable: true),
                    PlacaVehiculo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaIncidente = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EstacionIncidente = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CarrilIncidente = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TipoSolucion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescripcionSolucion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MontoAjuste = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RequiereReembolso = table.Column<bool>(type: "bit", nullable: false),
                    RequiereNuevaTargeta = table.Column<bool>(type: "bit", nullable: false),
                    CalificacionCliente = table.Column<int>(type: "int", nullable: true),
                    ComentarioCliente = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaCalificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ArchivosEvidencia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ObservacionesInternas = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TarjetaRFIDAfectadaNavigationId = table.Column<int>(type: "int", nullable: true),
                    TransaccionAfectadaNavigationId = table.Column<int>(type: "int", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketSoporte", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketSoporte_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketSoporte_TarjetasRFID_TarjetaRFIDAfectadaNavigationId",
                        column: x => x.TarjetaRFIDAfectadaNavigationId,
                        principalTable: "TarjetasRFID",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketSoporte_Transacciones_TransaccionAfectadaNavigationId",
                        column: x => x.TransaccionAfectadaNavigationId,
                        principalTable: "Transacciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClienteFacturaDetalle",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClienteFacturaId = table.Column<int>(type: "int", nullable: false),
                    TransaccionId = table.Column<int>(type: "int", nullable: true),
                    Concepto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cantidad = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnidadMedida = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DescuentoPorcentaje = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DescuentoMonto = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IVAPorcentaje = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IVAMonto = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FechaCruce = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EstacionOrigen = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CarrilOrigen = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlacaVehiculo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TipoVehiculo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TagRFID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClienteFacturaDetalle", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClienteFacturaDetalle_ClienteFactura_ClienteFacturaId",
                        column: x => x.ClienteFacturaId,
                        principalTable: "ClienteFactura",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClienteFacturaDetalle_Transacciones_TransaccionId",
                        column: x => x.TransaccionId,
                        principalTable: "Transacciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClientePago",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    ClienteFacturaId = table.Column<int>(type: "int", nullable: true),
                    NumeroOperacion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FechaPago = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FormaPago = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Banco = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Referencia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CuentaOrigen = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CuentaDestino = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaConfirmacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaAplicacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioRegistro = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsuarioConfirmacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ComprobantePago = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UUIDComplemento = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaTimbradoComplemento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    XMLComplemento = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientePago", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientePago_ClienteFactura_ClienteFacturaId",
                        column: x => x.ClienteFacturaId,
                        principalTable: "ClienteFactura",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientePago_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TicketSoporteHistorial",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketSoporteId = table.Column<int>(type: "int", nullable: false),
                    FechaAccion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Usuario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoAccion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstadoAnterior = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstadoNuevo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comentario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EsVisible = table.Column<bool>(type: "bit", nullable: false),
                    ArchivosAdjuntos = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DatosAdicionales = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketSoporteHistorial", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketSoporteHistorial_TicketSoporte_TicketSoporteId",
                        column: x => x.TicketSoporteId,
                        principalTable: "TicketSoporte",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TarjetasRFID_ClienteVehiculoId",
                table: "TarjetasRFID",
                column: "ClienteVehiculoId");

            migrationBuilder.CreateIndex(
                name: "IX_ClienteDocumento_ClienteId",
                table: "ClienteDocumento",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_ClienteFactura_ClienteId",
                table: "ClienteFactura",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_ClienteFacturaDetalle_ClienteFacturaId",
                table: "ClienteFacturaDetalle",
                column: "ClienteFacturaId");

            migrationBuilder.CreateIndex(
                name: "IX_ClienteFacturaDetalle_TransaccionId",
                table: "ClienteFacturaDetalle",
                column: "TransaccionId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientePago_ClienteFacturaId",
                table: "ClientePago",
                column: "ClienteFacturaId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientePago_ClienteId",
                table: "ClientePago",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_ClienteRecarga_ClienteId",
                table: "ClienteRecarga",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_ClienteRecarga_TarjetaRFIDId",
                table: "ClienteRecarga",
                column: "TarjetaRFIDId");

            migrationBuilder.CreateIndex(
                name: "IX_ClienteVehiculo_ClienteId",
                table: "ClienteVehiculo",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketSoporte_ClienteId",
                table: "TicketSoporte",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketSoporte_TarjetaRFIDAfectadaNavigationId",
                table: "TicketSoporte",
                column: "TarjetaRFIDAfectadaNavigationId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketSoporte_TransaccionAfectadaNavigationId",
                table: "TicketSoporte",
                column: "TransaccionAfectadaNavigationId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketSoporteHistorial_TicketSoporteId",
                table: "TicketSoporteHistorial",
                column: "TicketSoporteId");

            migrationBuilder.AddForeignKey(
                name: "FK_TarjetasRFID_ClienteVehiculo_ClienteVehiculoId",
                table: "TarjetasRFID",
                column: "ClienteVehiculoId",
                principalTable: "ClienteVehiculo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TarjetasRFID_ClienteVehiculo_ClienteVehiculoId",
                table: "TarjetasRFID");

            migrationBuilder.DropTable(
                name: "ClienteDocumento");

            migrationBuilder.DropTable(
                name: "ClienteFacturaDetalle");

            migrationBuilder.DropTable(
                name: "ClientePago");

            migrationBuilder.DropTable(
                name: "ClienteRecarga");

            migrationBuilder.DropTable(
                name: "ClienteVehiculo");

            migrationBuilder.DropTable(
                name: "TicketSoporteHistorial");

            migrationBuilder.DropTable(
                name: "ClienteFactura");

            migrationBuilder.DropTable(
                name: "TicketSoporte");

            migrationBuilder.DropIndex(
                name: "IX_TarjetasRFID_ClienteVehiculoId",
                table: "TarjetasRFID");

            migrationBuilder.DropColumn(
                name: "ClienteVehiculoId",
                table: "TarjetasRFID");

            migrationBuilder.DropColumn(
                name: "EnListaNegra",
                table: "TarjetasRFID");

            migrationBuilder.DropColumn(
                name: "FechaBloqueo",
                table: "TarjetasRFID");

            migrationBuilder.DropColumn(
                name: "MotivoBloqueo",
                table: "TarjetasRFID");

            migrationBuilder.DropColumn(
                name: "ObservacionesSeguridad",
                table: "TarjetasRFID");

            migrationBuilder.DropColumn(
                name: "TipoAsociacion",
                table: "TarjetasRFID");

            migrationBuilder.DropColumn(
                name: "UltimaActividad",
                table: "TarjetasRFID");

            migrationBuilder.DropColumn(
                name: "AlertaSaldoBajo",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "DiasCredito",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "DocumentosValidados",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "DomicilioFiscal",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "EmailFacturacion",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "EstadoCliente",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "FechaAprobacion",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "FechaCierre",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "FechaConsentimiento",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "FechaCorteCredito",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "FechaSuspension",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "FechaValidacionDocumentos",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "FechaVencimientoDocumentos",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "ModeloCuenta",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "MontoAlertaSaldo",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "MotivoCierre",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "MotivoSuspension",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "ObservacionesKYC",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "PeriodicidadFacturacion",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "PreferenciaFacturacion",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "RFC",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "RazonSocial",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "RegimenFiscal",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "SaldoMinimo",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "SerieFacturacion",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "TopeCredito",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "UsoCFDI",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "UsuarioAprobacion",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "UsuarioCreacion",
                table: "Clientes");
        }
    }
}
