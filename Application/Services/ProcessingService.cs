using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Application.Validators;
using AutoMapper;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class ProcessingService : IProcessingService
    {
        private readonly ITerpelClient _terpelClient;
        private readonly IFileParser _fileParser;
        private readonly IMapper _mapper;
        private readonly ILogger<ProcessingService> _logger;

        public ProcessingService(ITerpelClient terpelClient, IFileParser fileParser, IMapper mapper, ILogger<ProcessingService> logger)
        {
            _terpelClient = terpelClient;
            _fileParser = fileParser;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProcessResultDto> ProcessSyncAsync(ProcessingRequestDto request)
        {
            var txId = Guid.NewGuid().ToString();
            _logger.LogInformation("Procesando sincronamente. idTransaccion={TxId}", txId);

            var result = new ProcessResultDto { IdTransaccion = txId };

            // 1. Obtener URL real
            var finalUrl = await _terpelClient.GetDynamicUrlAsync(request.DynamicUrl);

            // 2. Descargar archivo
            using var stream = await _terpelClient.DownloadFileAsync(finalUrl);

            // 3. Parsear archivo
            var registros = await _fileParser.ParseAsync(stream, finalUrl);

            // 4. Validar cada registro
            var validator = new RegistroVentaValidator();
            foreach (var r in registros)
            {
                ValidationResult vr = await validator.ValidateAsync(r);
                if (vr.IsValid)
                {
                    result.RegistrosValidos.Add(r);
                }
                else
                {
                    result.RegistrosInvalidos.Add((r, vr.Errors.Select(e => e.ErrorMessage).ToList()));
                }
            }

            // 5. Log estructurado
            _logger.LogInformation("Procesamiento complete. idTransaccion={TxId} validos={Valid} invalidos={Invalid}", txId, result.RegistrosValidos.Count, result.RegistrosInvalidos.Count);

            return result;
        }

        public async Task<string> ProcessAsync(ProcessingRequestDto request)
        {
            var txId = Guid.NewGuid().ToString();
            _logger.LogInformation("Iniciando procesamiento asíncrono. idTransaccion={TxId}", txId);

            // Start background task (fire-and-forget)
            _ = Task.Run(async () =>
            {
                try
                {
                    var res = await ProcessSyncAsync(request);

                    // Simular callback de éxito (si se pasó CallbackUrl)
                    if (!string.IsNullOrWhiteSpace(request.CallbackUrl))
                    {
                        try
                        {
                            using var client = new System.Net.Http.HttpClient();
                            var json = System.Text.Json.JsonSerializer.Serialize(res);
                            var content = new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json");
                            await client.PostAsync(request.CallbackUrl, content);
                        }
                        catch (Exception cbEx)
                        {
                            _logger.LogError(cbEx, "Error enviando callback de éxito para idTransaccion={TxId}", txId);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error en procesamiento asíncrono idTransaccion={TxId}", txId);

                    // Intentar enviar callback de error
                    if (!string.IsNullOrWhiteSpace(request.CallbackUrl))
                    {
                        try
                        {
                            using var client = new System.Net.Http.HttpClient();
                            var payload = new { idTransaccion = txId, error = ex.Message };
                            var json = System.Text.Json.JsonSerializer.Serialize(payload);
                            var content = new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json");
                            await client.PostAsync(request.CallbackUrl, content);
                        }
                        catch (Exception cbEx)
                        {
                            _logger.LogError(cbEx, "Error enviando callback de error para idTransaccion={TxId}", txId);
                        }
                    }
                }
            });

            return txId;
        }
    }
}
