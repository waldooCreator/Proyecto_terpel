using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class OAuthTokenProvider : IOAuthTokenProvider
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<OAuthTokenProvider> _logger;

        private string _cachedToken = string.Empty;
        private DateTime _expiresAt = DateTime.MinValue;
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        public OAuthTokenProvider(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<OAuthTokenProvider> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            if (!string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _expiresAt)
            {
                return _cachedToken;
            }

            await _lock.WaitAsync();
            try
            {
                if (!string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _expiresAt)
                    return _cachedToken;

                var tokenUrl = _configuration["OAuth:TokenUrl"];
                var clientId = _configuration["OAuth:ClientId"];
                var clientSecret = _configuration["OAuth:ClientSecret"];

                if (string.IsNullOrWhiteSpace(tokenUrl) || string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
                {
                    _logger.LogWarning("OAuth configuration missing (TokenUrl/ClientId/ClientSecret).");
                    return string.Empty;
                }

                var client = _httpClientFactory.CreateClient();
                var req = new HttpRequestMessage(HttpMethod.Post, tokenUrl);
                var body = new StringContent($"grant_type=client_credentials&client_id={Uri.EscapeDataString(clientId)}&client_secret={Uri.EscapeDataString(clientSecret)}", Encoding.UTF8, "application/x-www-form-urlencoded");
                req.Content = body;

                var resp = await client.SendAsync(req);
                resp.EnsureSuccessStatusCode();

                var json = await resp.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                if (root.TryGetProperty("access_token", out var accessTokenElem))
                {
                    _cachedToken = accessTokenElem.GetString() ?? string.Empty;
                    var expiresIn = 3600;
                    if (root.TryGetProperty("expires_in", out var expiresInElem) && expiresInElem.TryGetInt32(out var e))
                    {
                        expiresIn = e;
                    }
                    _expiresAt = DateTime.UtcNow.AddSeconds(expiresIn - 30); // refresh margin
                    _logger.LogInformation("Obtained OAuth access token, expires in {ExpiresIn}s", expiresIn);
                    return _cachedToken;
                }

                _logger.LogWarning("Token response did not contain access_token: {Response}", json);
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obtaining OAuth token");
                throw;
            }
            finally
            {
                _lock.Release();
            }
        }
    }
}
