using FluentValidation;
using Application.DTOs;
using System.Globalization;

namespace Application.Validators
{
    public class RegistroVentaValidator : AbstractValidator<RegistroVentaDto>
    {
        public RegistroVentaValidator()
        {
            // Basic required validations — placeholders for Terpel rules
            RuleFor(x => x.CódigoEstación).NotEmpty();
            RuleFor(x => x.Estación).NotEmpty();
            RuleFor(x => x.Fecha).NotEmpty();
            RuleFor(x => x.Hora).NotEmpty();
            RuleFor(x => x.Producto).NotEmpty();
            RuleFor(x => x.Cantidad).NotEmpty();
            RuleFor(x => x.Precio).NotEmpty();

            // Numeric checks where applicable
            RuleFor(x => x.Cantidad).Must(BeDecimal).WithMessage("Cantidad debe ser numérico");
            RuleFor(x => x.Precio).Must(BeDecimal).WithMessage("Precio debe ser numérico");
            RuleFor(x => x.ValorVenta).Must(BeDecimal).When(x => !string.IsNullOrWhiteSpace(x.ValorVenta)).WithMessage("ValorVenta debe ser numérico");
        }

        private bool BeDecimal(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            return decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out _);
        }
    }
}
