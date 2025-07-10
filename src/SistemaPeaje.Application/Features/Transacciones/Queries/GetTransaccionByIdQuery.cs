using MediatR;
using SistemaPeaje.Application.DTOs;

namespace SistemaPeaje.Application.Features.Transacciones.Queries;

public class GetTransaccionByIdQuery : IRequest<TransaccionDto>
{
    public int Id { get; set; }
}
