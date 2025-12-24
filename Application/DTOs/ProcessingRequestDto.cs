namespace Application.DTOs
{
    public class ProcessingRequestDto
    {
        // Placeholder dynamic url provided by Terpel (daily)
        public string DynamicUrl { get; set; } = string.Empty;

        // Placeholder for auth type: "OAuth", "ApiKey", "Basic"
        public string AuthType { get; set; } = string.Empty;

        // For async mode, callback url to notify
        public string CallbackUrl { get; set; } = string.Empty;
    }
}
