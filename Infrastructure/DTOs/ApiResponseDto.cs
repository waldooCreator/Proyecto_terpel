using System;
using System.Text.Json.Serialization;

namespace Infrastructure.DTOs
{
    public class ApiResponseDto<T>
    {
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("data")]
        public T? Data { get; set; }

        [JsonPropertyName("success")]
        public bool? SuccessFlag { get; set; }

        [JsonIgnore]
        public bool? IsSuccess { get; set; }

        [JsonIgnore]
        public bool SuccessComputed =>
            (IsSuccess ?? SuccessFlag) == true ||
            string.Equals(Status, "ok", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(Status, "success", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(Status, "1", StringComparison.OrdinalIgnoreCase);
    }
}
