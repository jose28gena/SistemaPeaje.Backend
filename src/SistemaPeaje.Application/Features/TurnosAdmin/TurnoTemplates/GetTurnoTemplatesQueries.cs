using AutoMapper;
using MediatR;
using SistemaPeaje.Application.DTOs.TurnosAdmin;
using SistemaPeaje.Application.Mappings;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;

namespace SistemaPeaje.Application.Features.TurnosAdmin.TurnoTemplates;

public record GetTurnoTemplatesQuery : IRequest<List<TurnoTemplateDto>>
{
    public bool? SoloActivos { get; init; }
}

public class GetTurnoTemplatesHandler : IRequestHandler<GetTurnoTemplatesQuery, List<TurnoTemplateDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetTurnoTemplatesHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<TurnoTemplateDto>> Handle(GetTurnoTemplatesQuery request, CancellationToken cancellationToken)
    {
        var templates = await _unitOfWork.Repository<TurnoTemplate>()
            .GetAsync(t => !request.SoloActivos.HasValue || !request.SoloActivos.Value || t.EsActivo);

        return _mapper.Map<List<TurnoTemplateDto>>(templates);
    }
}

public record GetTurnoTemplateByIdQuery : IRequest<TurnoTemplateDto?>
{
    public int Id { get; init; }
}

public class GetTurnoTemplateByIdHandler : IRequestHandler<GetTurnoTemplateByIdQuery, TurnoTemplateDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetTurnoTemplateByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<TurnoTemplateDto?> Handle(GetTurnoTemplateByIdQuery request, CancellationToken cancellationToken)
    {
        var template = await _unitOfWork.Repository<TurnoTemplate>().GetByIdAsync(request.Id);
        return template != null ? _mapper.Map<TurnoTemplateDto>(template) : null;
    }
}

public record GetTurnoTemplatesAplicablesQuery : IRequest<List<TurnoTemplateDto>>
{
    public DayOfWeek DiaSemana { get; init; }
}

public class GetTurnoTemplatesAplicablesHandler : IRequestHandler<GetTurnoTemplatesAplicablesQuery, List<TurnoTemplateDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetTurnoTemplatesAplicablesHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<TurnoTemplateDto>> Handle(GetTurnoTemplatesAplicablesQuery request, CancellationToken cancellationToken)
    {
        var templates = await _unitOfWork.Repository<TurnoTemplate>()
            .GetAsync(t => t.EsActivo && EsAplicableParaDia(t, request.DiaSemana));

        return _mapper.Map<List<TurnoTemplateDto>>(templates);
    }

    private static bool EsAplicableParaDia(TurnoTemplate template, DayOfWeek diaSemana)
    {
        return diaSemana switch
        {
            DayOfWeek.Monday => template.Lunes,
            DayOfWeek.Tuesday => template.Martes,
            DayOfWeek.Wednesday => template.Miercoles,
            DayOfWeek.Thursday => template.Jueves,
            DayOfWeek.Friday => template.Viernes,
            DayOfWeek.Saturday => template.Sabado,
            DayOfWeek.Sunday => template.Domingo,
            _ => false
        };
    }
}
