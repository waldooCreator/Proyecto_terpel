namespace Application.DTOs
{
    // Respuesta del endpoint /api/v1/eds/consolidados-eds
    public class TerpelConsolidadoResponseDto
    {
        public string eds_id { get; set; } = string.Empty;
        public string nombre_eds { get; set; } = string.Empty;
        public int total_archivos { get; set; }
        public int urls_generadas { get; set; }
        public int expiracion_minutos { get; set; }
        public string fecha_generacion { get; set; } = string.Empty;
        public string fecha_expiracion { get; set; } = string.Empty;
        public List<TerpelArchivoDto> archivos { get; set; } = new();
    }

    public class TerpelArchivoDto
    {
        public string signed_url { get; set; } = string.Empty;
        public string nombre_archivo { get; set; } = string.Empty;
        public string ruta_gcs { get; set; } = string.Empty;
        public string fecha_generacion { get; set; } = string.Empty;
        public string fecha_expiracion_url { get; set; } = string.Empty;
    }
}
