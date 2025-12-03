using System.Threading.Tasks;
using Application.DTOs;
using Application.Validators;
using Xunit;

namespace Tests
{
    public class RegistroVentaValidatorTests
    {
        [Fact]
        public async Task Validator_Rejects_Invalid_Numeric_Fields()
        {
            var validator = new RegistroVentaValidator();
            var dto = new RegistroVentaDto
            {
                CódigoEstación = "",
                Estación = "",
                Fecha = "",
                Hora = "",
                Producto = "",
                Cantidad = "notnumeric",
                Precio = "NaN"
            };

            var result = await validator.ValidateAsync(dto);
            Assert.False(result.IsValid);
        }
    }
}
