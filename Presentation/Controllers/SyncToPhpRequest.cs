using System.Collections.Generic;

namespace Presentation.Controllers
{
    public class SyncToPhpRequest
    {
        public int EdsId { get; set; }
        public JobPayload Job { get; set; } = new();
        public ArchivoPayload Archivo { get; set; } = new();
        public List<Application.DTOs.RegistroVentaDto> Ventas { get; set; } = new();
    }

    public class JobPayload
    {
        public int EdsId { get; set; }
        public int TotalArchivos { get; set; }
        public int UrlsGeneradas { get; set; }
        public int ExpiracionMinutos { get; set; }
        public int Status { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class ArchivoPayload
    {
        public string? Nombre { get; set; }
        public string? NombreArchivo { get; set; }
        public string? SignedUrl { get; set; }
        public string? RutaGcs { get; set; }
        public string? FechaExpiracionUrl { get; set; }
        public int ParsedRows { get; set; }
        public string? ErrorMessage { get; set; }
        public string? Status { get; set; }
    }
}
