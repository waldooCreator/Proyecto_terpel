namespace Domain.Entities
{
    public class ConsolidadoArchivo
    {
        public int Id { get; set; }
        public int JobId { get; set; }
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
