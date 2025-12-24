using System.Text.Json.Serialization;

namespace Infrastructure.DTOs
{
    public class PhpVentaResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }
}
