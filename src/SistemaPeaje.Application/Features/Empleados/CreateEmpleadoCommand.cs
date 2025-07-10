using AutoMapper;
using MediatR;
using SistemaPeaje.Application.Mappings;
using SistemaPeaje.Core.Entities;
using SistemaPeaje.Core.Interfaces;

namespace SistemaPeaje.Application.Features.Empleados;

public record CreateEmpleadoCommand : IRequest<EmpleadoDto>
{
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string NumeroDocumento { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string Cargo { get; set; } = string.Empty;
    public int? EstacionId { get; set; }
}

public class CreateEmpleadoHandler : IRequestHandler<CreateEmpleadoCommand, EmpleadoDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateEmpleadoHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<EmpleadoDto> Handle(CreateEmpleadoCommand request, CancellationToken cancellationToken)
    {
        var empleado = new Empleado
        {
            Nombres = request.Nombres,
            Apellidos = request.Apellidos,
            NumeroDocumento = request.NumeroDocumento,
            Email = request.Email,
            Telefono = request.Telefono,
            Cargo = request.Cargo,
            EstacionId = request.EstacionId,
            FechaContratacion = DateTime.UtcNow,
            EsActivo = true
        };

        await _unitOfWork.Repository<Empleado>().AddAsync(empleado);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<EmpleadoDto>(empleado);
    }
}
