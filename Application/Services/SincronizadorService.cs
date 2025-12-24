using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Entities;
using Domain.Primitives;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class SincronizadorService : ISincronizadorService
    {
        private readonly IConsolidadoJobRepository _jobRepository;
        private readonly IConsolidadoArchivoRepository _archivoRepository;
        private readonly IVentaDetalladaRepository _ventaRepository;
        private readonly ILogger<SincronizadorService> _logger;

        public SincronizadorService(
            IConsolidadoJobRepository jobRepository,
            IConsolidadoArchivoRepository archivoRepository,
            IVentaDetalladaRepository ventaRepository,
            ILogger<SincronizadorService> logger)
        {
            _jobRepository = jobRepository;
            _archivoRepository = archivoRepository;
            _ventaRepository = ventaRepository;
            _logger = logger;
        }

        public async Task<OperationResult<int>> SincronizarAsync(ConsolidadoJob job, ConsolidadoArchivo archivo, IEnumerable<VentaDetallada> ventas, CancellationToken cancellationToken = default)
        {
            var ventasList = ventas as IList<VentaDetallada> ?? ventas.ToList();
            _logger.LogInformation("Iniciando sincronizacion: ventas={Count}", ventasList.Count);

            var jobResult = await _jobRepository.CrearJobAsync(job, cancellationToken);
            if (!jobResult.Success)
            {
                var message = jobResult.Message ?? "No se pudo crear el job";
                _logger.LogWarning("Sincronizacion detenida: {Message}", message);
                return OperationResult<int>.Fail(message);
            }

            var jobId = jobResult.Data;
            archivo.JobId = jobId;

            var archivoResult = await _archivoRepository.CrearArchivoAsync(archivo, cancellationToken);
            if (!archivoResult.Success)
            {
                var message = archivoResult.Message ?? "No se pudo registrar el archivo";
                _logger.LogWarning("Sincronizacion detenida: {Message}", message);
                return OperationResult<int>.Fail(message);
            }

            var archivoId = archivoResult.Data;
            foreach (var venta in ventasList)
            {
                venta.JobId = jobId;
                venta.ArchivoId = archivoId;
            }

            var ventasResult = await _ventaRepository.CrearVentasAsync(ventasList, cancellationToken);
            if (!ventasResult.Success)
            {
                var message = ventasResult.Message ?? "No se pudieron enviar las ventas";
                _logger.LogWarning("Sincronizacion incompleta: {Message}", message);
                return OperationResult<int>.Fail(message);
            }

            return OperationResult<int>.Ok(ventasResult.Data, ventasResult.Message);
        }
    }
}
