namespace Infrastructure.Options
{
    public class PhpApiOptions
    {
        public string BaseUrl { get; set; } = "https://esprinsas.com/API/";
        public string ApiKey { get; set; } = string.Empty;
        public int TimeoutSeconds { get; set; } = 30;
        public int RetryCount { get; set; } = 3;
    }
}
