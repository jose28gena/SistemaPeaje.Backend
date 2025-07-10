using AutoMapper;
using MediatR;
using SistemaPeaje.Application.DTOs;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;

namespace SistemaPeaje.Application.Features.TiposCliente;

public class UpdateTipoClienteCommand : IRequest<TipoClienteDto>
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public bool EstaExentoPago { get; set; }
    public decimal? DescuentoPorcentaje { get; set; }
    public bool RequiereValidacionDocumento { get; set; }
    public string? DocumentosRequeridos { get; set; }
    public int? VigenciaMeses { get; set; }
    public bool EsActivo { get; set; }
}

public class UpdateTipoClienteHandler : IRequestHandler<UpdateTipoClienteCommand, TipoClienteDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateTipoClienteHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<TipoClienteDto> Handle(UpdateTipoClienteCommand request, CancellationToken cancellationToken)
    {
        var tipoCliente = await _unitOfWork.Repository<TipoCliente>().GetByIdAsync(request.Id);
        if (tipoCliente == null)
            throw new ArgumentException($"TipoCliente con ID {request.Id} no encontrado");

        tipoCliente.Nombre = request.Nombre;
        tipoCliente.Descripcion = request.Descripcion;
        tipoCliente.EstaExentoPago = request.EstaExentoPago;
        tipoCliente.DescuentoPorcentaje = request.DescuentoPorcentaje;
        tipoCliente.RequiereValidacionDocumento = request.RequiereValidacionDocumento;
        tipoCliente.DocumentosRequeridos = request.DocumentosRequeridos;
        tipoCliente.VigenciaMeses = request.VigenciaMeses;
        tipoCliente.Activo = request.EsActivo;
        tipoCliente.FechaActualizacion = DateTime.UtcNow;

        await _unitOfWork.Repository<TipoCliente>().UpdateAsync(tipoCliente);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<TipoClienteDto>(tipoCliente);
    }
}
