using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    // Placeholder implementation ready to be extended with real auth mechanisms
    public class TerpelHttpService : ITerpelClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<TerpelHttpService> _logger;

        public TerpelHttpService(IHttpClientFactory httpClientFactory, ILogger<TerpelHttpService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public Task<string> GetDynamicUrlAsync(string dynamicUrlPlaceholder)
        {
            // Placeholder: in real usage, call the Terpel endpoint with auth to obtain the final URL
            _logger.LogInformation("GetDynamicUrlAsync called with placeholder={Placeholder}", dynamicUrlPlaceholder);

            // Return the placeholder as final URL for demo, or a dummy CSV data URL
            if (string.IsNullOrWhiteSpace(dynamicUrlPlaceholder))
            {
                return Task.FromResult("dummy://local/dummy.csv");
            }

            return Task.FromResult(dynamicUrlPlaceholder);
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

            // Otherwise perform a real HTTP GET (placeholder for auth handling)
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(fileUrl);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStreamAsync();
        }
    }
}
