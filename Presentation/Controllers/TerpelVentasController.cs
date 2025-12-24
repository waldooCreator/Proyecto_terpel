using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/terpel/ventas")]
    public class TerpelVentasController : ControllerBase
    {
        private readonly IProcessingService _processingService;
        private readonly ISincronizadorService _sincronizadorService;
        private readonly ILogger<TerpelVentasController> _logger;

        public TerpelVentasController(
            IProcessingService processingService,
            ISincronizadorService sincronizadorService,
            ILogger<TerpelVentasController> logger)
        {
            _processingService = processingService;
            _sincronizadorService = sincronizadorService;
            _logger = logger;
        }

        [HttpPost("sync")]
        public async Task<IActionResult> ProcessSync([FromBody] ProcessingRequestDto request)
        {
            _logger.LogInformation("/api/terpel/ventas/sync called");
            var result = await _processingService.ProcessSyncAsync(request);
            return Ok(result);
        }

        [HttpPost("async")]
        public async Task<IActionResult> ProcessAsync([FromBody] ProcessingRequestDto request)
        {
            _logger.LogInformation("/api/terpel/ventas/async called");
            var txId = await _processingService.ProcessAsync(request);
            return Accepted(new { idTransaccion = txId });
        }

        [HttpPost("sync-php")]
        public async Task<IActionResult> SyncToPhp([FromBody] SyncToPhpRequest request)
        {
            _logger.LogInformation("/api/terpel/ventas/sync-php called ventas={Count}", request.Ventas.Count);

            // Prefer EDS id provided by frontend; fallback to job payload or default 1
            var edsId = request.EdsId > 0
                ? request.EdsId
                : (request.Job?.EdsId > 0 ? request.Job.EdsId : 1);

            // Keep a single resolved EDS id for job and ventas payloads
            var resolvedEdsId = edsId;

            var job = new ConsolidadoJob
            {
                EdsId = resolvedEdsId,
                TotalArchivos = request.Job.TotalArchivos > 0 ? request.Job.TotalArchivos : 1,
                UrlsGeneradas = request.Job.UrlsGeneradas > 0 ? request.Job.UrlsGeneradas : 1,
                ExpiracionMinutos = request.Job.ExpiracionMinutos > 0 ? request.Job.ExpiracionMinutos : 60,
                Status = request.Job.Status <= 0 ? 1 : request.Job.Status,
                ErrorMessage = request.Job.ErrorMessage
            };

            var archivo = new ConsolidadoArchivo
            {
                NombreArchivo = string.IsNullOrWhiteSpace(request.Archivo.NombreArchivo)
                    ? (string.IsNullOrWhiteSpace(request.Archivo.Nombre) ? "ventas.csv" : request.Archivo.Nombre)
                    : request.Archivo.NombreArchivo,
                Nombre = string.IsNullOrWhiteSpace(request.Archivo.NombreArchivo)
                    ? (string.IsNullOrWhiteSpace(request.Archivo.Nombre) ? "ventas.csv" : request.Archivo.Nombre)
                    : request.Archivo.NombreArchivo,
                SignedUrl = request.Archivo.SignedUrl ?? request.Archivo.RutaGcs ?? request.Archivo.Nombre ?? string.Empty,
                RutaGcs = request.Archivo.RutaGcs ?? string.Empty,
                FechaExpiracionUrl = string.IsNullOrWhiteSpace(request.Archivo.FechaExpiracionUrl) ? "0" : request.Archivo.FechaExpiracionUrl,
                ParsedRows = request.Archivo.ParsedRows,
                ErrorMessage = request.Archivo.ErrorMessage ?? string.Empty,
                Status = string.IsNullOrWhiteSpace(request.Archivo.Status) ? "procesando" : request.Archivo.Status
            };

            var ventas = new List<VentaDetallada>();
            foreach (var v in request.Ventas)
            {
                ventas.Add(new VentaDetallada
                {
                    EdsId = resolvedEdsId,
                    Estacion = v.Estación,
                    CodigoEstacion = v.CódigoEstación,
                    CodigoPromotor = v.CódigoPromotor,
                    Promotor = v.Promotor,
                    Turno = v.Turno,
                    Fecha = v.Fecha,
                    Hora = v.Hora,
                    Isla = v.Isla,
                    Surtidor = v.Surtidor,
                    Cara = v.Cara,
                    Manguera = v.Manguera,
                    LecturaInicial = ParseDouble(v.LecturaInicial),
                    LecturaFinal = ParseDouble(v.LecturaFinal),
                    Tanque = v.Tanque,
                    TipoFactura = v.TipoFactura,
                    Prefijo = v.Prefijo,
                    Consecutivo = v.Consecutivo,
                    Producto = v.Producto,
                    Impoconsumo = ParseDouble(v.Impoconsumo),
                    PorcentajeIva = ParseDouble(v.IvaPorcentaje),
                    ValorIva = ParseDouble(v.ValorIva),
                    Precio = ParseDouble(v.Precio),
                    PrecioDiferencial = ParseDouble(v.PrecioDiferencial),
                    Cantidad = ParseDouble(v.Cantidad),
                    Unidad = v.Unidad,
                    Descuento = ParseDouble(v.Descuento),
                    ValorVenta = ParseDouble(v.ValorVenta),
                    MedioPago = v.MedioPago,
                    MedioIdentificacion = v.MedioIdentificacion,
                    Placa = v.Placa,
                    TipoDocumentoFe = v.TipoDocumentoFE,
                    NumeroDocumentoFe = v.NumeroDocumentoFE,
                    NombreCliente = v.NombreCliente,
                    Correo = v.Correo,
                    Kilometraje = v.Kilometraje,
                    Telefono = v.Telefono,
                    Direccion = v.Direccion,
                    TipoNegocio = v.TipoNegocio
                });
            }

            var result = await _sincronizadorService.SincronizarAsync(job, archivo, ventas);
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(new { enviados = result.Data, mensaje = result.Message });
        }

        private static double ParseDouble(string? value)
        {
            if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var d))
            {
                return d;
            }
            return 0d;
        }
    }
}
