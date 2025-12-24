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
                    var colKey = colName.Trim().ToUpperInvariant();
                    var raw = cols[c].Trim();
                    var value = raw;
                    // Map by name — supporting ALL 39+ fields from real CSV
                    switch (colKey)
                    {
                        // 28 campos originales
                        case "CODIGO ESTACION": dto.CódigoEstación = value; break;
                        case "ESTACION": dto.Estación = value; break;
                        case "CODIGO DEL PROMOTOR": dto.CódigoPromotor = value; break;
                        case "PROMOTOR": dto.Promotor = value; break;
                        case "TURNO": dto.Turno = value; break;
                        case "FECHA": dto.Fecha = value; break;
                        case "HORA": dto.Hora = value; break;
                        case "ISLA": dto.Isla = value; break;
                        case "SURTIDOR": dto.Surtidor = value; break;
                        case "CARA": dto.Cara = value; break;
                        case "MANGUERA": dto.Manguera = value; break;
                        case "LECTURA INICIAL": dto.LecturaInicial = CleanNumeric(value); break;
                        case "LECTURA FINAL": dto.LecturaFinal = CleanNumeric(value); break;
                        case "TANQUE": dto.Tanque = value; break;
                        case "TIPO FACTURA": dto.TipoFactura = value; break;
                        case "PREFIJO": dto.Prefijo = value; break;
                        case "CONSECUTIVO": dto.Consecutivo = value; break;
                        case "REFERENCIA": dto.Referencia = value; break;
                        case "PRODUCTO": dto.Producto = value; break;
                        case "IMPOCONSUMO": dto.Impoconsumo = CleanNumeric(value); break;
                        case "IVA PORCENTAJE": dto.IvaPorcentaje = CleanNumeric(value); break;
                        case "VALOR DEL IVA": dto.ValorIva = CleanNumeric(value); break;
                        case "PRECIO": dto.Precio = CleanNumeric(value); break;
                        case "PRECIO DIFERENCIAL": dto.PrecioDiferencial = CleanNumeric(value); break;
                        case "PRECIO_UNITARIO": dto.Precio = CleanNumeric(value); break;
                        case "PRECIO UNITARIO": dto.Precio = CleanNumeric(value); break;
                        case "VALOR_UNITARIO": dto.Precio = CleanNumeric(value); break;
                        case "VALOR UNITARIO": dto.Precio = CleanNumeric(value); break;
                        case "CANTIDAD": dto.Cantidad = CleanNumeric(value); break;
                        case "UNIDAD": dto.Unidad = value; break;
                        case "DESCUENTO": dto.Descuento = CleanNumeric(value); break;
                        case "VALOR VENTA": dto.ValorVenta = CleanNumeric(value); break;

                        // 11 campos adicionales del CSV real
                        case "MEDIO PAGO": dto.MedioPago = value; break;
                        case "MEDIO DE IDENTIFICACION": dto.MedioIdentificacion = value; break;
                        case "PLACA": dto.Placa = value; break;
                        case "TIPO DE DOCUMENTO FE": dto.TipoDocumentoFE = value; break;
                        case "NUMERO DE DOCUMENTO FE": dto.NumeroDocumentoFE = value; break;
                        case "NOMBRE CLIENTE": dto.NombreCliente = value; break;
                        case "CORREO": dto.Correo = value; break;
                        case "KILOMETRAJE": dto.Kilometraje = value; break;
                        case "TELEFONO": dto.Telefono = value; break;
                        case "DIRECCION": dto.Direccion = value; break;
                        case "TIPO DE NEGOCIO": dto.TipoNegocio = value; break;
                        
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

        private static string CleanNumeric(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;
            return value.Replace("$", string.Empty).Replace(",", string.Empty).Trim();
        }
    }
}
