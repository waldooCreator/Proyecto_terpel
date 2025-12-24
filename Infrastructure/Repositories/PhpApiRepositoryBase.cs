using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.DTOs;
using Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Repositories
{
    public abstract class PhpApiRepositoryBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        protected readonly PhpApiOptions Options;
        private readonly ILogger _logger;
        private readonly JsonSerializerOptions _serializerOptions;
        private static readonly System.Text.UTF8Encoding Utf8NoBom = new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        protected PhpApiRepositoryBase(IHttpClientFactory httpClientFactory, IOptions<PhpApiOptions> options, ILogger logger)
        {
            _httpClientFactory = httpClientFactory;
            Options = options.Value;
            _logger = logger;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        protected async Task<ApiResponseDto<T>?> PostAsync<T>(string path, object payload, CancellationToken cancellationToken)
        {
            var client = _httpClientFactory.CreateClient("PhpApi");
            var json = JsonSerializer.Serialize(payload, _serializerOptions);
            _logger.LogInformation("PHP API request path={Path} payload={Payload}", path, Truncate(json, 500));

            for (int attempt = 1; attempt <= Options.RetryCount; attempt++)
            {
                using var content = new StringContent(json, Utf8NoBom, "application/json");
                // Avoid chunked transfer; PHP shared hosts may fail parsing php://input when chunked
                content.Headers.ContentLength = Utf8NoBom.GetByteCount(json);

                using var request = new HttpRequestMessage(HttpMethod.Post, path)
                {
                    Version = HttpVersion.Version11,
                    Content = content
                };
                request.Headers.TransferEncodingChunked = false;

                try
                {
                    var response = await client.SendAsync(request, cancellationToken);
                    var body = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("PHP API response status={Status} length={Length}", (int)response.StatusCode, body?.Length ?? 0);

                    if (response.IsSuccessStatusCode)
                    {
                        // Primero intentamos deserializar como ApiResponseDto<T>
                        var apiResponse = body != null ? JsonSerializer.Deserialize<ApiResponseDto<T>>(body, _serializerOptions) : null;
                        if (apiResponse != null)
                        {
                            if (apiResponse.Data == null && apiResponse.SuccessComputed)
                            {
                                var asDirectFromEnvelope = body != null ? JsonSerializer.Deserialize<T>(body, _serializerOptions) : default;
                                if (asDirectFromEnvelope != null)
                                {
                                    apiResponse.Data = asDirectFromEnvelope;
                                }
                            }

                            if (apiResponse.Data != null || apiResponse.IsSuccess.HasValue || apiResponse.SuccessFlag.HasValue || apiResponse.SuccessComputed)
                            {
                                return apiResponse;
                            }
                        }

                        // Si no calza en el envoltorio esperado, deserializar el cuerpo directamente como T y envolverlo
                        var direct = body != null ? JsonSerializer.Deserialize<T>(body, _serializerOptions) : default;
                        if (direct != null)
                        {
                            return new ApiResponseDto<T>
                            {
                                Data = direct,
                                IsSuccess = true,
                                SuccessFlag = true,
                                Status = "ok"
                            };
                        }

                        _logger.LogWarning("PHP API deserialization failed for path={Path} body={Body}", path, Truncate(body ?? string.Empty));
                        return apiResponse;
                    }

                    if ((int)response.StatusCode >= 500 && attempt < Options.RetryCount)
                    {
                        _logger.LogWarning("PHP API 5xx (attempt {Attempt}/{Retry}) status={Status} body={Body}", attempt, Options.RetryCount, (int)response.StatusCode, Truncate(body ?? string.Empty));
                        continue;
                    }

                    _logger.LogError("PHP API error status={Status} body={Body}", (int)response.StatusCode, Truncate(body ?? string.Empty));
                    break;
                }
                catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
                {
                    _logger.LogWarning(ex, "PHP API timeout (attempt {Attempt}/{Retry})", attempt, Options.RetryCount);
                    if (attempt == Options.RetryCount)
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "PHP API error (attempt {Attempt}/{Retry})", attempt, Options.RetryCount);
                    if (attempt == Options.RetryCount)
                    {
                        throw;
                    }
                }
            }

            return null;
        }

        private string Truncate(string body, int maxLength = 400)
        {
            if (string.IsNullOrEmpty(body)) return string.Empty;
            return body.Length <= maxLength ? body : body.Substring(0, maxLength);
        }
    }
}
