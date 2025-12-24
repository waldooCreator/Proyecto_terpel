using FluentValidation;
using Application.DTOs;
using System.Globalization;

namespace Application.Validators
{
    public class RegistroVentaValidator : AbstractValidator<RegistroVentaDto>
    {
        public RegistroVentaValidator()
        {
            // Basic required validations for 28 original fields
            RuleFor(x => x.CódigoEstación).NotEmpty();
            RuleFor(x => x.Estación).NotEmpty();
            RuleFor(x => x.Fecha).NotEmpty();
            RuleFor(x => x.Hora).NotEmpty();
            RuleFor(x => x.Producto).NotEmpty();
            RuleFor(x => x.Cantidad).NotEmpty();

            // Numeric checks where applicable
            RuleFor(x => x.Cantidad).Must(BeDecimal).WithMessage("Cantidad debe ser numérico");
            RuleFor(x => x.ValorVenta).Must(BeDecimal).When(x => !string.IsNullOrWhiteSpace(x.ValorVenta)).WithMessage("ValorVenta debe ser numérico");

            // Precio: si viene vacío o no numérico, intentar calcularlo o dejar 0 y no bloquear el registro
            RuleFor(x => x).Custom((x, ctx) =>
            {
                var precioClean = Clean(x.Precio);
                if (decimal.TryParse(precioClean, NumberStyles.Any, CultureInfo.InvariantCulture, out var precioDecimal))
                {
                    x.Precio = precioDecimal.ToString(CultureInfo.InvariantCulture);
                    return;
                }

                var valorVentaClean = Clean(x.ValorVenta);
                var cantidadClean = Clean(x.Cantidad);
                if (decimal.TryParse(valorVentaClean, NumberStyles.Any, CultureInfo.InvariantCulture, out var vv)
                    && decimal.TryParse(cantidadClean, NumberStyles.Any, CultureInfo.InvariantCulture, out var qty)
                    && qty != 0)
                {
                    var calc = vv / qty;
                    x.Precio = calc.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    x.Precio = "0";
                }
            });

            // Los 11 campos adicionales son opcionales — no validation required
            // MedioPago, MedioIdentificacion, Placa, TipoDocumentoFE, NumeroDocumentoFE,
            // NombreCliente, Correo, Kilometraje, Telefono, Direccion, TipoNegocio
        }

        private bool BeDecimal(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            return decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out _);
        }

        private string Clean(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;
            return value.Replace("$", string.Empty).Replace(",", string.Empty).Trim();
        }
    }
}
