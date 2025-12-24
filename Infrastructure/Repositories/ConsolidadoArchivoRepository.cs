using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Primitives;
using Domain.Repositories;
using Infrastructure.DTOs;
using Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Repositories
{
    public class ConsolidadoArchivoRepository : PhpApiRepositoryBase, IConsolidadoArchivoRepository
    {
        private readonly ILogger<ConsolidadoArchivoRepository> _logger;

        public ConsolidadoArchivoRepository(
            System.Net.Http.IHttpClientFactory httpClientFactory,
            IOptions<PhpApiOptions> options,
            ILogger<ConsolidadoArchivoRepository> logger) : base(httpClientFactory, options, logger)
        {
            _logger = logger;
        }

        public async Task<OperationResult<int>> CrearArchivoAsync(ConsolidadoArchivo archivo, CancellationToken cancellationToken = default)
        {
            var payload = new ConsolidadoArchivoRequestDto
            {
                JobId = archivo.JobId,
                NombreArchivo = string.IsNullOrWhiteSpace(archivo.NombreArchivo) ? (archivo.Nombre ?? "ventas.csv") : archivo.NombreArchivo,
                SignedUrl = archivo.SignedUrl ?? string.Empty,
                RutaGcs = archivo.RutaGcs ?? string.Empty,
                FechaExp = archivo.FechaExpiracionUrl ?? "0",
                ParsedRows = archivo.ParsedRows,
                Status = string.IsNullOrWhiteSpace(archivo.Status) ? "procesando" : archivo.Status,
                ErrorMessage = archivo.ErrorMessage ?? string.Empty
            };

            var response = await PostAsync<ConsolidadoArchivoResponseDto>("consolidado_archivo_create.php", payload, cancellationToken);

            if (response?.SuccessComputed == true && response.Data != null && (response.Data.ArchivoId > 0 || response.Data.Id > 0))
            {
                var archivoId = response.Data.ArchivoId > 0 ? response.Data.ArchivoId : response.Data.Id;
                _logger.LogInformation("Consolidado archivo creado id={Id}", archivoId);
                return OperationResult<int>.Ok(archivoId, response.Message ?? response.Data.Status);
            }

            var message = response?.Message ?? "No se pudo crear el consolidado archivo";
            _logger.LogWarning("Fallo creando consolidado archivo: {Message}", message);
            return OperationResult<int>.Fail(message);
        }
    }
}
