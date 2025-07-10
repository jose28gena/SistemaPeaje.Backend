using FluentValidation;
using SistemaPeaje.Application.Features.Transacciones.Commands;

namespace SistemaPeaje.Application.Validators;

public class CreateTransaccionCommandValidator : AbstractValidator<CreateTransaccionCommand>
{
    public CreateTransaccionCommandValidator()
    {
        RuleFor(x => x.EstacionId)
            .GreaterThan(0)
            .WithMessage("El ID de estación debe ser mayor a 0");

        RuleFor(x => x.CarrilId)
            .GreaterThan(0)
            .WithMessage("El ID de carril debe ser mayor a 0");

        RuleFor(x => x.TipoVehiculoId)
            .GreaterThan(0)
            .WithMessage("El ID de tipo de vehículo debe ser mayor a 0");

        RuleFor(x => x.TipoPagoId)
            .GreaterThan(0)
            .WithMessage("El ID de tipo de pago debe ser mayor a 0");

        RuleFor(x => x.Monto)
            .GreaterThan(0)
            .WithMessage("El monto debe ser mayor a 0");

        RuleFor(x => x.PlacaVehiculo)
            .MaximumLength(10)
            .When(x => !string.IsNullOrEmpty(x.PlacaVehiculo))
            .WithMessage("La placa del vehículo no puede tener más de 10 caracteres");

        RuleFor(x => x.TagRFID)
            .MaximumLength(50)
            .When(x => !string.IsNullOrEmpty(x.TagRFID))
            .WithMessage("El tag RFID no puede tener más de 50 caracteres");

        RuleFor(x => x.Observaciones)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Observaciones))
            .WithMessage("Las observaciones no pueden tener más de 500 caracteres");
    }
}
