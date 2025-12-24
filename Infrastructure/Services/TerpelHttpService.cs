using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    // Servicio que consume la API real de Terpel con Bearer token
    public class TerpelHttpService : ITerpelClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<TerpelHttpService> _logger;
        private readonly Application.Interfaces.IOAuthTokenProvider _oauthProvider;
        private readonly IConfiguration _configuration;

        public TerpelHttpService(IHttpClientFactory httpClientFactory, ILogger<TerpelHttpService> logger, Application.Interfaces.IOAuthTokenProvider oauthProvider, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _oauthProvider = oauthProvider;
            _configuration = configuration;
        }

        public async Task<string> GetDynamicUrlAsync(string dynamicUrlPlaceholder)
        {
            _logger.LogInformation("GetDynamicUrlAsync called with placeholder={Placeholder}", dynamicUrlPlaceholder);

            // Si es el dummy, retornar como antes
            if (!string.IsNullOrWhiteSpace(dynamicUrlPlaceholder) && dynamicUrlPlaceholder.StartsWith("dummy://"))
            {
                return dynamicUrlPlaceholder;
            }

            // Si se especifica "real" o está vacío, llamar al endpoint real de Terpel
            if (string.IsNullOrWhiteSpace(dynamicUrlPlaceholder) || dynamicUrlPlaceholder.Equals("real", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var terpelApiUrl = _configuration["TerpelApi:ConsolidadosUrl"];
                    var bearerToken = _configuration["TerpelApi:BearerToken"];

                    if (string.IsNullOrWhiteSpace(terpelApiUrl) || string.IsNullOrWhiteSpace(bearerToken))
                    {
                        _logger.LogWarning("TerpelApi configuration missing (ConsolidadosUrl or BearerToken)");
                        return "dummy://local/dummy.csv";
                    }

                    var handler = new HttpClientHandler
                    {
                        AutomaticDecompression = System.Net.DecompressionMethods.All
                    };
                    var client = new HttpClient(handler);
                    
                    client.DefaultRequestHeaders.Add("User-Agent", "PostmanRuntime/7.49.1");
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip,deflate,br");
                    client.DefaultRequestHeaders.Add("Connection", "keep-alive");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

                    _logger.LogInformation("Calling Terpel API: {Url}", terpelApiUrl);
                    var response = await client.GetAsync(terpelApiUrl);
                    response.EnsureSuccessStatusCode();

                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var consolidado = JsonSerializer.Deserialize<TerpelConsolidadoResponseDto>(jsonContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (consolidado?.archivos != null && consolidado.archivos.Count > 0)
                    {
                        var signedUrl = consolidado.archivos[0].signed_url;
                        _logger.LogInformation("Obtained signed URL from Terpel API. Expires: {Expiration}", consolidado.fecha_expiracion);
                        return signedUrl;
                    }

                    _logger.LogWarning("Terpel API returned no files");
                    return "dummy://local/dummy.csv";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error calling Terpel API");
                    return "dummy://local/dummy.csv";
                }
            }

            return dynamicUrlPlaceholder;
        }

        public async Task<Stream> DownloadFileAsync(string fileUrl)
        {
            _logger.LogInformation("DownloadFileAsync called with url={Url}", fileUrl);

            if (fileUrl.StartsWith("dummy://local/"))
            {
                // Return a MemoryStream with a small CSV content as placeholder
                var csv = "CódigoEstación,Estación,CódigoPromotor,Promotor,Turno,Fecha,Hora,Isla,Surtidor,Cara,Manguera,LecturaInicial,LecturaFinal,Tanque,TipoFactura,Prefijo,Consecutivo,Referencia,Producto,Impoconsumo,IvaPorcentaje,ValorIva,Precio,PrecioDiferencial,Cantidad,Unidad,Descuento,ValorVenta\n" +
                          "001,EstacionX,PR001,PromotorA,1,2025-11-30,08:00,1,1,1,1,100,150,T01,Factura,F,123,REF123,Gasolina,0,19,5.0,2.5,0,50,L,0,125";

                var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
                return new MemoryStream(bytes);
            }

            // Otherwise perform a real HTTP GET
            // Note: Signed URLs from Google Cloud Storage already contain authentication
            // DO NOT add OAuth token for GCS signed URLs
            var client = _httpClientFactory.CreateClient();
            try
            {
                // Signed URLs don't need additional authentication
                _logger.LogInformation("Downloading file from signed URL: {Url}", fileUrl.Substring(0, Math.Min(100, fileUrl.Length)));
                
                var response = await client.GetAsync(fileUrl);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStreamAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading file from {Url}", fileUrl);
                throw;
            }
        }
    }
}
