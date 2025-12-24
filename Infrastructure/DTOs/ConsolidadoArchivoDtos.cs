using System.Text.Json.Serialization;

namespace Infrastructure.DTOs
{
    public class ConsolidadoArchivoRequestDto
    {
        [JsonPropertyName("job_id")]
        public int JobId { get; set; }

        [JsonPropertyName("nombre_archivo")]
        public string NombreArchivo { get; set; } = string.Empty;

        [JsonPropertyName("signed_url")]
        public string SignedUrl { get; set; } = string.Empty;

        [JsonPropertyName("ruta_gcs")]
        public string RutaGcs { get; set; } = string.Empty;

        [JsonPropertyName("fecha_expiracion_url")]
        public string FechaExp { get; set; } = string.Empty;

        [JsonPropertyName("parsed_rows")]
        public int ParsedRows { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("error_message")]
        public string ErrorMessage { get; set; } = string.Empty;
    }

    public class ConsolidadoArchivoResponseDto
    {
        [JsonPropertyName("archivo_id")]
        public int ArchivoId { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }
}
