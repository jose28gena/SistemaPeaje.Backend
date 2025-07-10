using AutoMapper;
using MediatR;
using SistemaPeaje.Application.Mappings;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;

namespace SistemaPeaje.Application.Features.Empleados;

public record UpdateEmpleadoCommand : IRequest<EmpleadoDto>
{
    public int Id { get; set; }
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string NumeroDocumento { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string Cargo { get; set; } = string.Empty;
    public int? EstacionId { get; set; }
    public bool EsActivo { get; set; } = true;
}

public class UpdateEmpleadoHandler : IRequestHandler<UpdateEmpleadoCommand, EmpleadoDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateEmpleadoHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<EmpleadoDto> Handle(UpdateEmpleadoCommand request, CancellationToken cancellationToken)
    {
        var empleado = await _unitOfWork.Repository<Empleado>().GetByIdAsync(request.Id);
        if (empleado == null)
            throw new KeyNotFoundException($"Empleado with ID {request.Id} not found");

        empleado.Nombres = request.Nombres;
        empleado.Apellidos = request.Apellidos;
        empleado.NumeroDocumento = request.NumeroDocumento;
        empleado.Email = request.Email;
        empleado.Telefono = request.Telefono;
        empleado.Cargo = request.Cargo;
        empleado.EstacionId = request.EstacionId;
        empleado.EsActivo = request.EsActivo;

        await _unitOfWork.Repository<Empleado>().UpdateAsync(empleado);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<EmpleadoDto>(empleado);
    }
}
