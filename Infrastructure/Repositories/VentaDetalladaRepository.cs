using System.Collections.Generic;
using System.Linq;
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
    public class VentaDetalladaRepository : PhpApiRepositoryBase, IVentaDetalladaRepository
    {
        private readonly ILogger<VentaDetalladaRepository> _logger;

        public VentaDetalladaRepository(
            System.Net.Http.IHttpClientFactory httpClientFactory,
            IOptions<PhpApiOptions> options,
            ILogger<VentaDetalladaRepository> logger) : base(httpClientFactory, options, logger)
        {
            _logger = logger;
        }

        public async Task<OperationResult<int>> CrearVentaAsync(VentaDetallada venta, CancellationToken cancellationToken = default)
        {
            var payload = ToRequestDto(venta);
            var response = await PostAsync<VentaDetalladaResponseDto>("venta_detallada_create.php", payload, cancellationToken);

            if (response?.SuccessComputed == true && response.Data != null && response.Data.Id > 0)
            {
                _logger.LogInformation("Venta detallada enviada id={Id}", response.Data.Id);
                return OperationResult<int>.Ok(response.Data.Id, response.Message ?? response.Data.Status);
            }

            var message = response?.Message ?? "No se pudo crear la venta detallada";
            _logger.LogWarning("Fallo enviando venta detallada job={Job} archivo={Archivo}: {Message}", venta.JobId, venta.ArchivoId, message);
            return OperationResult<int>.Fail(message);
        }

        public async Task<OperationResult<int>> CrearVentasAsync(IEnumerable<VentaDetallada> ventas, CancellationToken cancellationToken = default)
        {
            var list = ventas as IList<VentaDetallada> ?? ventas.ToList();
            var payload = new { ventas = list.Select(ToRequestDto).ToList() };

            var response = await PostAsync<PhpVentaResponse>("venta_detallada_create.php", payload, cancellationToken);

            if (response?.Data != null && response.Data.Success)
            {
                var count = response.Data.Count;
                _logger.LogInformation("Ventas detalladas enviadas en bulk: {Count}", count);
                return OperationResult<int>.Ok(count, response.Message ?? response.Status ?? $"Ventas enviadas: {count}");
            }

            var msg = response?.Message ?? "No se pudieron enviar las ventas";
            _logger.LogWarning("Fallo enviando ventas en bulk: {Message}", msg);
            return OperationResult<int>.Fail(msg);
        }

        private static VentaDetalladaRequestDto ToRequestDto(VentaDetallada v)
        {
            return new VentaDetalladaRequestDto
            {
                JobId = v.JobId,
                ArchivoId = v.ArchivoId,
                EdsId = v.EdsId,
                Estacion = v.Estacion ?? string.Empty,
                CodigoEstacion = v.CodigoEstacion ?? string.Empty,
                CodigoPromotor = v.CodigoPromotor ?? string.Empty,
                Promotor = v.Promotor ?? string.Empty,
                Turno = v.Turno ?? string.Empty,
                Fecha = string.IsNullOrWhiteSpace(v.Fecha) ? "1900-01-01" : v.Fecha,
                Hora = string.IsNullOrWhiteSpace(v.Hora) ? "00:00:00" : v.Hora,
                Isla = v.Isla ?? string.Empty,
                Surtidor = v.Surtidor ?? string.Empty,
                Cara = v.Cara ?? string.Empty,
                Manguera = v.Manguera ?? string.Empty,
                LecturaInicial = v.LecturaInicial,
                LecturaFinal = v.LecturaFinal,
                Tanque = v.Tanque ?? string.Empty,
                TipoFactura = v.TipoFactura ?? string.Empty,
                Prefijo = v.Prefijo ?? string.Empty,
                Consecutivo = v.Consecutivo ?? string.Empty,
                ConsecutivoAnulada = v.ConsecutivoAnulada ?? string.Empty,
                Producto = v.Producto ?? string.Empty,
                Impoconsumo = v.Impoconsumo,
                PorcentajeIva = v.PorcentajeIva,
                ValorIva = v.ValorIva,
                Precio = v.Precio,
                PrecioDiferencial = v.PrecioDiferencial,
                Cantidad = v.Cantidad,
                Unidad = v.Unidad ?? string.Empty,
                Descuento = v.Descuento,
                ValorVenta = v.ValorVenta,
                MedioPago = v.MedioPago ?? string.Empty,
                MedioIdentificacion = v.MedioIdentificacion ?? string.Empty,
                Placa = v.Placa ?? string.Empty,
                TipoDocumentoFe = v.TipoDocumentoFe ?? string.Empty,
                NumeroDocumentoFe = v.NumeroDocumentoFe ?? string.Empty,
                NombreCliente = v.NombreCliente ?? string.Empty,
                Correo = v.Correo ?? string.Empty,
                Kilometraje = v.Kilometraje ?? string.Empty,
                Telefono = v.Telefono ?? string.Empty,
                Direccion = v.Direccion ?? string.Empty,
                TipoNegocio = v.TipoNegocio ?? string.Empty
            };
        }
    }
}
