using System.Text.Json.Serialization;

namespace Infrastructure.DTOs
{
    public class ConsolidadoJobRequestDto
    {
        [JsonPropertyName("eds_id")]
        public int EdsId { get; set; }

        [JsonPropertyName("total_archivos")]
        public int TotalArchivos { get; set; }

        [JsonPropertyName("urls_generadas")]
        public int UrlsGeneradas { get; set; }

        [JsonPropertyName("expiracion_minutos")]
        public int ExpiracionMinutos { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("error_message")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ErrorMessage { get; set; }
    }

    public class ConsolidadoJobResponseDto
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("job_id")]
        public int JobId { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }
}
