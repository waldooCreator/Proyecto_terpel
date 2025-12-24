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
    public class ConsolidadoJobRepository : PhpApiRepositoryBase, IConsolidadoJobRepository
    {
        private readonly ILogger<ConsolidadoJobRepository> _logger;

        public ConsolidadoJobRepository(
            System.Net.Http.IHttpClientFactory httpClientFactory,
            IOptions<PhpApiOptions> options,
            ILogger<ConsolidadoJobRepository> logger) : base(httpClientFactory, options, logger)
        {
            _logger = logger;
        }

        public async Task<OperationResult<int>> CrearJobAsync(ConsolidadoJob job, CancellationToken cancellationToken = default)
        {
            var payload = new ConsolidadoJobRequestDto
            {
                EdsId = job.EdsId,
                TotalArchivos = job.TotalArchivos,
                UrlsGeneradas = job.UrlsGeneradas,
                ExpiracionMinutos = job.ExpiracionMinutos,
                Status = job.Status.ToString(),
                ErrorMessage = job.ErrorMessage
            };

            var response = await PostAsync<ConsolidadoJobResponseDto>("consolidado_job_create.php", payload, cancellationToken);

            if (response?.Data is { Success: true, JobId: > 0 })
            {
                _logger.LogInformation("Consolidado job created id={Id}", response.Data.JobId);
                return OperationResult<int>.Ok(response.Data.JobId, response.Data.Message ?? response.Message);
            }

            if (response?.SuccessComputed == true && response?.Data is { JobId: > 0 })
            {
                _logger.LogInformation("Consolidado job created id={Id}", response.Data.JobId);
                return OperationResult<int>.Ok(response.Data.JobId, response.Data.Message ?? response.Message);
            }

            if (response?.Data == null)
            {
                _logger.LogWarning("Consolidado job response without data. Raw: success={Success} status={Status} message={Message}", response?.SuccessComputed, response?.Status, response?.Message);
            }

            var message = response?.Message ?? "No se pudo crear el consolidado job";
            _logger.LogWarning("Fallo creando consolidado job: {Message}", message);
            return OperationResult<int>.Fail(message);
        }
    }
}
