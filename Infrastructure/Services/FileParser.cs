using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class FileParser : IFileParser
    {
        private readonly ILogger<FileParser> _logger;

        public FileParser(ILogger<FileParser> logger)
        {
            _logger = logger;
        }

        public async Task<List<RegistroVentaDto>> ParseAsync(Stream fileStream, string fileName)
        {
            // Very small, robust parser that supports CSV with header matching our field names
            var registros = new List<RegistroVentaDto>();

            using var reader = new StreamReader(fileStream, Encoding.UTF8, true, 1024, leaveOpen: true);
            var content = await reader.ReadToEndAsync();
            var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length == 0) return registros;

            var header = lines[0].Split(',').Select(h => h.Trim()).ToArray();

            for (int i = 1; i < lines.Length; i++)
            {
                var cols = lines[i].Split(',');
                var dto = new RegistroVentaDto();
                for (int c = 0; c < Math.Min(header.Length, cols.Length); c++)
                {
                    var colName = header[c];
                    var value = cols[c].Trim();
                    // Map by name — only supporting the exact 28 fields
                    switch (colName)
                    {
                        case "CódigoEstación": dto.CódigoEstación = value; break;
                        case "Estación": dto.Estación = value; break;
                        case "CódigoPromotor": dto.CódigoPromotor = value; break;
                        case "Promotor": dto.Promotor = value; break;
                        case "Turno": dto.Turno = value; break;
                        case "Fecha": dto.Fecha = value; break;
                        case "Hora": dto.Hora = value; break;
                        case "Isla": dto.Isla = value; break;
                        case "Surtidor": dto.Surtidor = value; break;
                        case "Cara": dto.Cara = value; break;
                        case "Manguera": dto.Manguera = value; break;
                        case "LecturaInicial": dto.LecturaInicial = value; break;
                        case "LecturaFinal": dto.LecturaFinal = value; break;
                        case "Tanque": dto.Tanque = value; break;
                        case "TipoFactura": dto.TipoFactura = value; break;
                        case "Prefijo": dto.Prefijo = value; break;
                        case "Consecutivo": dto.Consecutivo = value; break;
                        case "Referencia": dto.Referencia = value; break;
                        case "Producto": dto.Producto = value; break;
                        case "Impoconsumo": dto.Impoconsumo = value; break;
                        case "IvaPorcentaje": dto.IvaPorcentaje = value; break;
                        case "ValorIva": dto.ValorIva = value; break;
                        case "Precio": dto.Precio = value; break;
                        case "PrecioDiferencial": dto.PrecioDiferencial = value; break;
                        case "Cantidad": dto.Cantidad = value; break;
                        case "Unidad": dto.Unidad = value; break;
                        case "Descuento": dto.Descuento = value; break;
                        case "ValorVenta": dto.ValorVenta = value; break;
                        default:
                            // ignore unknown columns
                            break;
                    }
                }

                registros.Add(dto);
            }

            _logger.LogInformation("Parsed {Count} registros from file {FileName}", registros.Count, fileName);
            return registros;
        }
    }
}
