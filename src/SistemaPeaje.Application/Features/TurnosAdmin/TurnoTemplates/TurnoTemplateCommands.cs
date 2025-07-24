using AutoMapper;
using MediatR;
using SistemaPeaje.Application.DTOs.TurnosAdmin;
using SistemaPeaje.Application.Mappings;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;

namespace SistemaPeaje.Application.Features.TurnosAdmin.TurnoTemplates;

public record CreateTurnoTemplateCommand : IRequest<TurnoTemplateDto>
{
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public TimeSpan HoraInicio { get; set; }
    public TimeSpan HoraFin { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public bool PermiteHorasExtras { get; set; } = true;
    public int MaximoHorasExtras { get; set; } = 2;
    public decimal FactorHoraExtra { get; set; } = 1.5m;
    public int MinutosDescanso { get; set; } = 30;
    public TimeSpan? HoraDescansoInicio { get; set; }
    public bool Lunes { get; set; } = true;
    public bool Martes { get; set; } = true;
    public bool Miercoles { get; set; } = true;
    public bool Jueves { get; set; } = true;
    public bool Viernes { get; set; } = true;
    public bool Sabado { get; set; } = true;
    public bool Domingo { get; set; } = true;
}

public class CreateTurnoTemplateHandler : IRequestHandler<CreateTurnoTemplateCommand, TurnoTemplateDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateTurnoTemplateHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<TurnoTemplateDto> Handle(CreateTurnoTemplateCommand request, CancellationToken cancellationToken)
    {
        // Validaciones
        if (request.HoraInicio >= request.HoraFin)
            throw new ArgumentException("La hora de inicio debe ser menor a la hora de fin");

        if (string.IsNullOrWhiteSpace(request.Nombre))
            throw new ArgumentException("El nombre del template es requerido");

        // Verificar que no exista otro template con el mismo nombre
        var templateExistente = await _unitOfWork.Repository<TurnoTemplate>()
            .GetAsync(t => t.Nombre.ToLower() == request.Nombre.ToLower());

        if (templateExistente.Any())
            throw new ArgumentException($"Ya existe un template con el nombre '{request.Nombre}'");

        var duracionMinutos = (int)(request.HoraFin - request.HoraInicio).TotalMinutes;

        var template = new TurnoTemplate
        {
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            HoraInicio = request.HoraInicio,
            HoraFin = request.HoraFin,
            DuracionMinutos = duracionMinutos,
            Tipo = request.Tipo,
            EsActivo = true,
            PermiteHorasExtras = request.PermiteHorasExtras,
            MaximoHorasExtras = request.MaximoHorasExtras,
            FactorHoraExtra = request.FactorHoraExtra,
            MinutosDescanso = request.MinutosDescanso,
            HoraDescansoInicio = request.HoraDescansoInicio,
            Lunes = request.Lunes,
            Martes = request.Martes,
            Miercoles = request.Miercoles,
            Jueves = request.Jueves,
            Viernes = request.Viernes,
            Sabado = request.Sabado,
            Domingo = request.Domingo
        };

        await _unitOfWork.Repository<TurnoTemplate>().AddAsync(template);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<TurnoTemplateDto>(template);
    }
}

public record UpdateTurnoTemplateCommand : IRequest<TurnoTemplateDto>
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public TimeSpan HoraInicio { get; set; }
    public TimeSpan HoraFin { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public bool EsActivo { get; set; } = true;
    public bool PermiteHorasExtras { get; set; } = true;
    public int MaximoHorasExtras { get; set; } = 2;
    public decimal FactorHoraExtra { get; set; } = 1.5m;
    public int MinutosDescanso { get; set; } = 30;
    public TimeSpan? HoraDescansoInicio { get; set; }
    public bool Lunes { get; set; } = true;
    public bool Martes { get; set; } = true;
    public bool Miercoles { get; set; } = true;
    public bool Jueves { get; set; } = true;
    public bool Viernes { get; set; } = true;
    public bool Sabado { get; set; } = true;
    public bool Domingo { get; set; } = true;
}

public class UpdateTurnoTemplateHandler : IRequestHandler<UpdateTurnoTemplateCommand, TurnoTemplateDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateTurnoTemplateHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<TurnoTemplateDto> Handle(UpdateTurnoTemplateCommand request, CancellationToken cancellationToken)
    {
        var template = await _unitOfWork.Repository<TurnoTemplate>().GetByIdAsync(request.Id);
        if (template == null)
            throw new KeyNotFoundException($"Template con ID {request.Id} no encontrado");

        // Validaciones
        if (request.HoraInicio >= request.HoraFin)
            throw new ArgumentException("La hora de inicio debe ser menor a la hora de fin");

        if (string.IsNullOrWhiteSpace(request.Nombre))
            throw new ArgumentException("El nombre del template es requerido");

        // Verificar que no exista otro template con el mismo nombre (excluyendo el actual)
        var templateExistente = await _unitOfWork.Repository<TurnoTemplate>()
            .GetAsync(t => t.Nombre.ToLower() == request.Nombre.ToLower() && t.Id != request.Id);

        if (templateExistente.Any())
            throw new ArgumentException($"Ya existe otro template con el nombre '{request.Nombre}'");

        var duracionMinutos = (int)(request.HoraFin - request.HoraInicio).TotalMinutes;

        // Actualizar propiedades
        template.Nombre = request.Nombre;
        template.Descripcion = request.Descripcion;
        template.HoraInicio = request.HoraInicio;
        template.HoraFin = request.HoraFin;
        template.DuracionMinutos = duracionMinutos;
        template.Tipo = request.Tipo;
        template.EsActivo = request.EsActivo;
        template.PermiteHorasExtras = request.PermiteHorasExtras;
        template.MaximoHorasExtras = request.MaximoHorasExtras;
        template.FactorHoraExtra = request.FactorHoraExtra;
        template.MinutosDescanso = request.MinutosDescanso;
        template.HoraDescansoInicio = request.HoraDescansoInicio;
        template.Lunes = request.Lunes;
        template.Martes = request.Martes;
        template.Miercoles = request.Miercoles;
        template.Jueves = request.Jueves;
        template.Viernes = request.Viernes;
        template.Sabado = request.Sabado;
        template.Domingo = request.Domingo;

        await _unitOfWork.Repository<TurnoTemplate>().UpdateAsync(template);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<TurnoTemplateDto>(template);
    }
}

public record DeleteTurnoTemplateCommand : IRequest
{
    public int Id { get; set; }
}

public class DeleteTurnoTemplateHandler : IRequestHandler<DeleteTurnoTemplateCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTurnoTemplateHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteTurnoTemplateCommand request, CancellationToken cancellationToken)
    {
        var template = await _unitOfWork.Repository<TurnoTemplate>().GetByIdAsync(request.Id);
        if (template == null)
            throw new KeyNotFoundException($"Template con ID {request.Id} no encontrado");

        // Verificar que no tenga asignaciones futuras
        var tieneAsignacionesFuturas = await _unitOfWork.Repository<TurnoAsignacion>()
            .GetAsync(a => a.TurnoTemplateId == request.Id && a.FechaTurno > DateTime.Now);

        if (tieneAsignacionesFuturas.Any())
            throw new InvalidOperationException("No se puede eliminar el template porque tiene asignaciones futuras");

        // En lugar de eliminar, marcamos como inactivo
        template.EsActivo = false;
        await _unitOfWork.Repository<TurnoTemplate>().UpdateAsync(template);
        await _unitOfWork.SaveChangesAsync();
    }
}

public record DuplicarTurnoTemplateCommand : IRequest<TurnoTemplateDto>
{
    public int TemplateIdOriginal { get; set; }
    public string NuevoNombre { get; set; } = string.Empty;
}

public class DuplicarTurnoTemplateHandler : IRequestHandler<DuplicarTurnoTemplateCommand, TurnoTemplateDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public DuplicarTurnoTemplateHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<TurnoTemplateDto> Handle(DuplicarTurnoTemplateCommand request, CancellationToken cancellationToken)
    {
        var templateOriginal = await _unitOfWork.Repository<TurnoTemplate>().GetByIdAsync(request.TemplateIdOriginal);
        if (templateOriginal == null)
            throw new KeyNotFoundException($"Template original con ID {request.TemplateIdOriginal} no encontrado");

        // Verificar que no exista otro template con el nuevo nombre
        var templateExistente = await _unitOfWork.Repository<TurnoTemplate>()
            .GetAsync(t => t.Nombre.ToLower() == request.NuevoNombre.ToLower());

        if (templateExistente.Any())
            throw new ArgumentException($"Ya existe un template con el nombre '{request.NuevoNombre}'");

        var nuevoTemplate = new TurnoTemplate
        {
            Nombre = request.NuevoNombre,
            Descripcion = templateOriginal.Descripcion,
            HoraInicio = templateOriginal.HoraInicio,
            HoraFin = templateOriginal.HoraFin,
            DuracionMinutos = templateOriginal.DuracionMinutos,
            Tipo = templateOriginal.Tipo,
            EsActivo = true,
            PermiteHorasExtras = templateOriginal.PermiteHorasExtras,
            MaximoHorasExtras = templateOriginal.MaximoHorasExtras,
            FactorHoraExtra = templateOriginal.FactorHoraExtra,
            MinutosDescanso = templateOriginal.MinutosDescanso,
            HoraDescansoInicio = templateOriginal.HoraDescansoInicio,
            Lunes = templateOriginal.Lunes,
            Martes = templateOriginal.Martes,
            Miercoles = templateOriginal.Miercoles,
            Jueves = templateOriginal.Jueves,
            Viernes = templateOriginal.Viernes,
            Sabado = templateOriginal.Sabado,
            Domingo = templateOriginal.Domingo
        };

        await _unitOfWork.Repository<TurnoTemplate>().AddAsync(nuevoTemplate);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<TurnoTemplateDto>(nuevoTemplate);
    }
}

/// <summary>
/// Command para actualizar configuración de turnos
/// </summary>
public record UpdateConfiguracionTurnosCommand(
    int MaxHorasSemanales,
    int MinDescansoEntreUturnos,
    bool PermitirHorasExtras,
    decimal MaxHorasExtrasDiarias,
    bool RequiereAprobacionCambios
) : IRequest<ConfiguracionTurnosDto>;

public class UpdateConfiguracionTurnosHandler : IRequestHandler<UpdateConfiguracionTurnosCommand, ConfiguracionTurnosDto>
{
    public async Task<ConfiguracionTurnosDto> Handle(UpdateConfiguracionTurnosCommand request, CancellationToken cancellationToken)
    {
        // Implementación básica - retorna configuración actualizada
        return new ConfiguracionTurnosDto
        {
            MaxHorasSemanales = request.MaxHorasSemanales,
            MinDescansoEntreTurnos = request.MinDescansoEntreUturnos,
            PermitirHorasExtras = request.PermitirHorasExtras,
            MaxHorasExtrasDiarias = request.MaxHorasExtrasDiarias,
            RequiereAprobacionCambios = request.RequiereAprobacionCambios
        };
    }
}

/// <summary>
/// DTO para configuración de turnos
/// </summary>
public class ConfiguracionTurnosDto
{
    public int MaxHorasSemanales { get; set; }
    public int MinDescansoEntreTurnos { get; set; }
    public bool PermitirHorasExtras { get; set; }
    public decimal MaxHorasExtrasDiarias { get; set; }
    public bool RequiereAprobacionCambios { get; set; }
}
