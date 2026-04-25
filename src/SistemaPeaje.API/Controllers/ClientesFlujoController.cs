using Microsoft.AspNetCore.Mvc;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;
using SistemaPeaje.Application.DTOs;

namespace SistemaPeaje.API.Controllers;

[ApiController]
[Route("api/clientes-flujo")]
//[Authorize] // Comentado temporalmente para desarrollo
public class ClientesFlujoController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public ClientesFlujoController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// 1. ALTA DE CLIENTE - Crear entidad cliente con datos fiscales y de contacto
    /// </summary>
    [HttpPost("alta")]
    public async Task<ActionResult<ClienteCompletaDto>> AltaCliente(AltaClienteRequest request)
    {
        try
        {
            // Validar duplicidad por RFC + Razón Social
            if (!string.IsNullOrEmpty(request.RFC))
            {
                var clientes = await _unitOfWork.Repository<Cliente>().GetAllAsync();
                var clienteExistente = clientes.FirstOrDefault(c => c.RFC == request.RFC && c.RazonSocial == request.RazonSocial);
                
                if (clienteExistente != null)
                {
                    return BadRequest($"Ya existe un cliente con RFC {request.RFC} y razón social {request.RazonSocial}");
                }
            }

            var cliente = new Cliente
            {
                // Datos básicos
                Nombres = request.Nombres,
                Apellidos = request.Apellidos,
                NumeroDocumento = request.NumeroDocumento,
                TipoDocumento = request.TipoDocumento,
                Email = request.Email,
                Telefono = request.Telefono,
                Direccion = request.Direccion,
                FechaNacimiento = request.FechaNacimiento,
                
                // Datos fiscales
                RFC = request.RFC,
                RazonSocial = request.RazonSocial,
                RegimenFiscal = request.RegimenFiscal,
                DomicilioFiscal = request.DomicilioFiscal,
                EmailFacturacion = request.EmailFacturacion,
                UsoCFDI = request.UsoCFDI,
                PreferenciaFacturacion = request.PreferenciaFacturacion ?? "PUE",
                
                // Estado inicial
                EstadoCliente = "Prospecto",
                DocumentosValidados = false,
                
                // Configuración por defecto
                ModeloCuenta = request.ModeloCuenta ?? "Prepago",
                AlertaSaldoBajo = true,
                MontoAlertaSaldo = 50,
                PeriodicidadFacturacion = "Mensual",
                
                // Auditoría
                UsuarioCreacion = request.UsuarioCreacion,
                FechaConsentimiento = request.FechaConsentimiento ?? DateTime.UtcNow,
                TipoClienteId = request.TipoClienteId,
                FechaCreacion = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Cliente>().AddAsync(cliente);
            await _unitOfWork.SaveChangesAsync();

            var clienteDto = await ObtenerClienteCompleto(cliente.Id);
            
            return CreatedAtAction(nameof(ObtenerClienteCompleto), new { id = cliente.Id }, clienteDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al crear cliente: {ex.Message}");
        }
    }

    /// <summary>
    /// 2. VALIDACIÓN KYC - Cambiar estado y validar documentos
    /// </summary>
    [HttpPost("{id}/kyc/aprobar")]
    public async Task<ActionResult> AprobarKYC(int id, AprobarKYCRequest request)
    {
        var cliente = await _unitOfWork.Repository<Cliente>().GetByIdAsync(id);
        if (cliente == null) return NotFound();

        cliente.EstadoCliente = "Aprobado";
        cliente.DocumentosValidados = true;
        cliente.FechaValidacionDocumentos = DateTime.UtcNow;
        cliente.FechaAprobacion = DateTime.UtcNow;
        cliente.UsuarioAprobacion = request.UsuarioAprobacion;
        cliente.ObservacionesKYC = request.Observaciones;
        cliente.FechaActualizacion = DateTime.UtcNow;

        await _unitOfWork.Repository<Cliente>().UpdateAsync(cliente);
        await _unitOfWork.SaveChangesAsync();

        return Ok(new { mensaje = "Cliente aprobado exitosamente", estado = cliente.EstadoCliente });
    }

    [HttpPost("{id}/kyc/rechazar")]
    public async Task<ActionResult> RechazarKYC(int id, RechazarKYCRequest request)
    {
        var cliente = await _unitOfWork.Repository<Cliente>().GetByIdAsync(id);
        if (cliente == null) return NotFound();

        cliente.EstadoCliente = "En validación";
        cliente.DocumentosValidados = false;
        cliente.ObservacionesKYC = request.MotivoRechazo;
        cliente.FechaActualizacion = DateTime.UtcNow;

        await _unitOfWork.Repository<Cliente>().UpdateAsync(cliente);
        await _unitOfWork.SaveChangesAsync();

        return Ok(new { mensaje = "Documentos rechazados", motivo = request.MotivoRechazo });
    }

    /// <summary>
    /// 3. CONFIGURACIÓN COMERCIAL
    /// </summary>
    [HttpPut("{id}/configuracion-comercial")]
    public async Task<ActionResult> ConfigurarComercial(int id, ConfiguracionComercialRequest request)
    {
        var cliente = await _unitOfWork.Repository<Cliente>().GetByIdAsync(id);
        if (cliente == null) return NotFound();

        cliente.ModeloCuenta = request.ModeloCuenta;
        cliente.TopeCredito = request.TopeCredito;
        cliente.DiasCredito = request.DiasCredito;
        cliente.FechaCorteCredito = request.FechaCorteCredito;
        cliente.SaldoMinimo = request.SaldoMinimo;
        cliente.AlertaSaldoBajo = request.AlertaSaldoBajo;
        cliente.MontoAlertaSaldo = request.MontoAlertaSaldo;
        cliente.PeriodicidadFacturacion = request.PeriodicidadFacturacion;
        cliente.SerieFacturacion = request.SerieFacturacion;
        cliente.FechaActualizacion = DateTime.UtcNow;

        await _unitOfWork.Repository<Cliente>().UpdateAsync(cliente);
        await _unitOfWork.SaveChangesAsync();

        return Ok(new { mensaje = "Configuración comercial actualizada" });
    }

    /// <summary>
    /// 4. ASOCIAR VEHÍCULO
    /// </summary>
    [HttpPost("{id}/vehiculos")]
    public async Task<ActionResult<ClienteVehiculoDto>> AsociarVehiculo(int id, AsociarVehiculoRequest request)
    {
        var cliente = await _unitOfWork.Repository<Cliente>().GetByIdAsync(id);
        if (cliente == null) return NotFound();

        // Validar que la placa no esté duplicada
        var vehiculos = await _unitOfWork.Repository<ClienteVehiculo>().GetAllAsync();
        var vehiculoExistente = vehiculos.FirstOrDefault(v => v.Placa == request.Placa && v.Activo);
        
        if (vehiculoExistente != null)
        {
            return BadRequest($"La placa {request.Placa} ya está registrada");
        }

        var vehiculo = new ClienteVehiculo
        {
            ClienteId = id,
            Placa = request.Placa,
            Marca = request.Marca,
            Modelo = request.Modelo,
            Año = request.Año,
            Color = request.Color,
            VIN = request.VIN,
            ClaseVehiculo = request.ClaseVehiculo,
            SubClaseVehiculo = request.SubClaseVehiculo,
            Propietario = request.Propietario,
            EsPrincipal = request.EsPrincipal,
            FechaRegistro = DateTime.UtcNow,
            FechaVencimientoDocumentos = request.FechaVencimientoDocumentos,
            FechaCreacion = DateTime.UtcNow
        };

        await _unitOfWork.Repository<ClienteVehiculo>().AddAsync(vehiculo);
        await _unitOfWork.SaveChangesAsync();

        var vehiculoDto = new ClienteVehiculoDto
        {
            Id = vehiculo.Id,
            ClienteId = vehiculo.ClienteId,
            Placa = vehiculo.Placa,
            Marca = vehiculo.Marca,
            Modelo = vehiculo.Modelo,
            Año = vehiculo.Año,
            Color = vehiculo.Color,
            ClaseVehiculo = vehiculo.ClaseVehiculo,
            SubClaseVehiculo = vehiculo.SubClaseVehiculo,
            EsPrincipal = vehiculo.EsPrincipal,
            Estado = vehiculo.Estado,
            FechaRegistro = vehiculo.FechaRegistro
        };

        return CreatedAtAction(nameof(ObtenerVehiculo), new { clienteId = id, vehiculoId = vehiculo.Id }, vehiculoDto);
    }

    /// <summary>
    /// 5. SUSPENDER/REACTIVAR CLIENTE
    /// </summary>
    [HttpPost("{id}/suspender")]
    public async Task<ActionResult> SuspenderCliente(int id, SuspenderClienteRequest request)
    {
        var cliente = await _unitOfWork.Repository<Cliente>().GetByIdAsync(id);
        if (cliente == null) return NotFound();

        cliente.EstadoCliente = "Suspendido";
        cliente.FechaSuspension = DateTime.UtcNow;
        cliente.MotivoSuspension = request.Motivo;
        cliente.FechaActualizacion = DateTime.UtcNow;

        // Bloquear todas las tarjetas del cliente
        var todasTarjetas = await _unitOfWork.Repository<TarjetaRFID>().GetAllAsync();
        var tarjetas = todasTarjetas.Where(t => t.ClienteId == id && t.Estado == "Activa");
        
        foreach (var tarjeta in tarjetas)
        {
            tarjeta.Estado = "Bloqueada";
            tarjeta.FechaBloqueo = DateTime.UtcNow;
            tarjeta.MotivoBloqueo = "Cliente suspendido";
            tarjeta.EnListaNegra = true;
            await _unitOfWork.Repository<TarjetaRFID>().UpdateAsync(tarjeta);
        }

        await _unitOfWork.Repository<Cliente>().UpdateAsync(cliente);
        await _unitOfWork.SaveChangesAsync();

        return Ok(new { mensaje = "Cliente suspendido", tarjetasBloqueadas = tarjetas.Count() });
    }

    [HttpPost("{id}/reactivar")]
    public async Task<ActionResult> ReactivarCliente(int id)
    {
        var cliente = await _unitOfWork.Repository<Cliente>().GetByIdAsync(id);
        if (cliente == null) return NotFound();

        cliente.EstadoCliente = "Aprobado";
        cliente.FechaSuspension = null;
        cliente.MotivoSuspension = null;
        cliente.FechaActualizacion = DateTime.UtcNow;

        await _unitOfWork.Repository<Cliente>().UpdateAsync(cliente);
        await _unitOfWork.SaveChangesAsync();

        return Ok(new { mensaje = "Cliente reactivado" });
    }

    /// <summary>
    /// 6. CERRAR/ARCHIVAR CLIENTE
    /// </summary>
    [HttpPost("{id}/cerrar")]
    public async Task<ActionResult> CerrarCliente(int id, CerrarClienteRequest request)
    {
        var cliente = await _unitOfWork.Repository<Cliente>().GetByIdAsync(id);
        if (cliente == null) return NotFound();

        // Verificar saldos pendientes
        var todasTarjetas = await _unitOfWork.Repository<TarjetaRFID>().GetAllAsync();
        var tarjetas = todasTarjetas.Where(t => t.ClienteId == id);
        
        var saldoTotal = tarjetas.Sum(t => t.Saldo);
        
        if (saldoTotal > 0 && !request.ForzarCierre)
        {
            return BadRequest($"El cliente tiene saldo pendiente de ${saldoTotal:F2}. Use forzarCierre=true para proceder.");
        }

        cliente.EstadoCliente = "Cerrado";
        cliente.FechaCierre = DateTime.UtcNow;
        cliente.MotivoCierre = request.Motivo;
        cliente.Activo = false;
        cliente.FechaActualizacion = DateTime.UtcNow;

        // Anular todas las tarjetas
        foreach (var tarjeta in tarjetas)
        {
            tarjeta.Estado = "Vencida";
            tarjeta.Activo = false;
            await _unitOfWork.Repository<TarjetaRFID>().UpdateAsync(tarjeta);
        }

        await _unitOfWork.Repository<Cliente>().UpdateAsync(cliente);
        await _unitOfWork.SaveChangesAsync();

        return Ok(new { 
            mensaje = "Cliente cerrado", 
            saldoDevuelto = saldoTotal,
            tarjetasAnuladas = tarjetas.Count() 
        });
    }

    /// <summary>
    /// CONSULTAS Y REPORTES
    /// </summary>
    [HttpGet("{id}/completo")]
    public async Task<ActionResult<ClienteCompletaDto>> ObtenerClienteCompleto(int id)
    {
        var cliente = await _unitOfWork.Repository<Cliente>().GetByIdAsync(id);
        if (cliente == null) return NotFound();

        // Obtener datos relacionados
        var todosVehiculos = await _unitOfWork.Repository<ClienteVehiculo>().GetAllAsync();
        var vehiculos = todosVehiculos.Where(v => v.ClienteId == id && v.Activo);
        
        var todasTarjetas = await _unitOfWork.Repository<TarjetaRFID>().GetAllAsync();
        var tarjetas = todasTarjetas.Where(t => t.ClienteId == id);
        
        var todosDocumentos = await _unitOfWork.Repository<ClienteDocumento>().GetAllAsync();
        var documentos = todosDocumentos.Where(d => d.ClienteId == id && d.Activo);
        
        var todosTickets = await _unitOfWork.Repository<TicketSoporte>().GetAllAsync();
        var tickets = todosTickets.Where(t => t.ClienteId == id && t.Estado != "Cerrado");

        var clienteDto = new ClienteCompletaDto
        {
            Id = cliente.Id,
            Nombres = cliente.Nombres,
            Apellidos = cliente.Apellidos,
            NumeroDocumento = cliente.NumeroDocumento,
            TipoDocumento = cliente.TipoDocumento,
            Email = cliente.Email,
            Telefono = cliente.Telefono,
            Direccion = cliente.Direccion,
            FechaNacimiento = cliente.FechaNacimiento,
            RFC = cliente.RFC,
            RazonSocial = cliente.RazonSocial,
            EstadoCliente = cliente.EstadoCliente,
            ModeloCuenta = cliente.ModeloCuenta,
            DocumentosValidados = cliente.DocumentosValidados,
            FechaCreacion = cliente.FechaCreacion,
            FechaActualizacion = cliente.FechaActualizacion,
            Activo = cliente.Activo,
            
            // Resumen
            Resumen = new ResumenClienteDto
            {
                TotalVehiculos = vehiculos.Count(),
                TotalTarjetas = tarjetas.Count(),
                TarjetasActivas = tarjetas.Count(t => t.Estado == "Activa"),
                TarjetasBloqueadas = tarjetas.Count(t => t.Estado == "Bloqueada"),
                SaldoTotal = tarjetas.Sum(t => t.Saldo),
                DocumentosPendientes = documentos.Count(d => d.EstadoValidacion == "Pendiente"),
                DocumentosAprobados = documentos.Count(d => d.EstadoValidacion == "Aprobado"),
                TicketsAbiertos = tickets.Count(),
                UltimaActividad = tarjetas.Max(t => t.UltimaActividad),
                RequiereAtencion = cliente.EstadoCliente == "Suspendido" || tickets.Any()
            }
        };

        return Ok(clienteDto);
    }

    [HttpGet("por-estado/{estado}")]
    public async Task<ActionResult<List<ClienteCompletaDto>>> ObtenerClientesPorEstado(string estado)
    {
        var todosClientes = await _unitOfWork.Repository<Cliente>().GetAllAsync();
        var clientes = todosClientes.Where(c => c.EstadoCliente == estado);

        var clientesDto = clientes.Select(c => new ClienteCompletaDto
        {
            Id = c.Id,
            Nombres = c.Nombres,
            Apellidos = c.Apellidos,
            Email = c.Email,
            EstadoCliente = c.EstadoCliente,
            FechaCreacion = c.FechaCreacion,
            FechaAprobacion = c.FechaAprobacion
        }).ToList();

        return Ok(clientesDto);
    }

    [HttpGet("{clienteId}/vehiculos/{vehiculoId}")]
    public async Task<ActionResult<ClienteVehiculoDto>> ObtenerVehiculo(int clienteId, int vehiculoId)
    {
        var todosVehiculos = await _unitOfWork.Repository<ClienteVehiculo>().GetAllAsync();
        var vehiculo = todosVehiculos.FirstOrDefault(v => v.Id == vehiculoId && v.ClienteId == clienteId);
        
        if (vehiculo == null) return NotFound();

        var vehiculoDto = new ClienteVehiculoDto
        {
            Id = vehiculo.Id,
            ClienteId = vehiculo.ClienteId,
            Placa = vehiculo.Placa,
            Marca = vehiculo.Marca,
            Estado = vehiculo.Estado
        };

        return Ok(vehiculoDto);
    }
}

// Request DTOs
public class AltaClienteRequest
{
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string? NumeroDocumento { get; set; }
    public string? TipoDocumento { get; set; }
    public string? Email { get; set; }
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    public DateTime? FechaNacimiento { get; set; }
    public string? RFC { get; set; }
    public string? RazonSocial { get; set; }
    public string? RegimenFiscal { get; set; }
    public string? DomicilioFiscal { get; set; }
    public string? EmailFacturacion { get; set; }
    public string? UsoCFDI { get; set; }
    public string? PreferenciaFacturacion { get; set; }
    public string? ModeloCuenta { get; set; }
    public int? TipoClienteId { get; set; }
    public string? UsuarioCreacion { get; set; }
    public DateTime? FechaConsentimiento { get; set; }
}

public class AprobarKYCRequest
{
    public string UsuarioAprobacion { get; set; } = string.Empty;
    public string? Observaciones { get; set; }
}

public class RechazarKYCRequest
{
    public string MotivoRechazo { get; set; } = string.Empty;
}

public class ConfiguracionComercialRequest
{
    public string ModeloCuenta { get; set; } = "Prepago";
    public decimal? TopeCredito { get; set; }
    public int? DiasCredito { get; set; }
    public DateTime? FechaCorteCredito { get; set; }
    public decimal? SaldoMinimo { get; set; }
    public bool AlertaSaldoBajo { get; set; } = true;
    public decimal? MontoAlertaSaldo { get; set; }
    public string? PeriodicidadFacturacion { get; set; }
    public string? SerieFacturacion { get; set; }
}

public class AsociarVehiculoRequest
{
    public string Placa { get; set; } = string.Empty;
    public string? Marca { get; set; }
    public string? Modelo { get; set; }
    public int? Año { get; set; }
    public string? Color { get; set; }
    public string? VIN { get; set; }
    public string ClaseVehiculo { get; set; } = string.Empty;
    public string SubClaseVehiculo { get; set; } = string.Empty;
    public string? Propietario { get; set; }
    public bool EsPrincipal { get; set; }
    public DateTime? FechaVencimientoDocumentos { get; set; }
}

public class SuspenderClienteRequest
{
    public string Motivo { get; set; } = string.Empty;
}

public class CerrarClienteRequest
{
    public string Motivo { get; set; } = string.Empty;
    public bool ForzarCierre { get; set; } = false;
}
