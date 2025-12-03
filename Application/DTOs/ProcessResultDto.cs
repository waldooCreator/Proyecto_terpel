using System.Collections.Generic;

namespace Application.DTOs
{
    public class ProcessResultDto
    {
        public string IdTransaccion { get; set; }
        public List<RegistroVentaDto> RegistrosValidos { get; set; } = new();
        public List<(RegistroVentaDto Registro, List<string> Errores)> RegistrosInvalidos { get; set; } = new();
    }
}
