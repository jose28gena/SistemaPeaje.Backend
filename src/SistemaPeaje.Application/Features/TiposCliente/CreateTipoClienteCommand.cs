using AutoMapper;
using MediatR;
using SistemaPeaje.Application.DTOs;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;

namespace SistemaPeaje.Application.Features.TiposCliente;

public class CreateTipoClienteCommand : IRequest<TipoClienteDto>
{
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public bool EstaExentoPago { get; set; }
    public decimal? DescuentoPorcentaje { get; set; }
    public bool RequiereValidacionDocumento { get; set; }
    public string? DocumentosRequeridos { get; set; }
    public int? VigenciaMeses { get; set; }
    public bool EsActivo { get; set; } = true;
}

public class CreateTipoClienteHandler : IRequestHandler<CreateTipoClienteCommand, TipoClienteDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateTipoClienteHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<TipoClienteDto> Handle(CreateTipoClienteCommand request, CancellationToken cancellationToken)
    {
        var tipoCliente = new TipoCliente
        {
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            EstaExentoPago = request.EstaExentoPago,
            DescuentoPorcentaje = request.DescuentoPorcentaje,
            RequiereValidacionDocumento = request.RequiereValidacionDocumento,
            DocumentosRequeridos = request.DocumentosRequeridos,
            VigenciaMeses = request.VigenciaMeses,
            Activo = request.EsActivo,
            FechaCreacion = DateTime.UtcNow
        };

        await _unitOfWork.Repository<TipoCliente>().AddAsync(tipoCliente);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<TipoClienteDto>(tipoCliente);
    }
}
