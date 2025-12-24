using System.Text.Json.Serialization;

namespace Infrastructure.DTOs
{
    public class VentaDetalladaRequestDto
    {
        [JsonPropertyName("job_id")]
        public int JobId { get; set; }

        [JsonPropertyName("archivo_id")]
        public int ArchivoId { get; set; }

        [JsonPropertyName("eds_id")]
        public int EdsId { get; set; }

        [JsonPropertyName("estacion")]
        public string Estacion { get; set; } = string.Empty;

        [JsonPropertyName("codigo_estacion")]
        public string CodigoEstacion { get; set; } = string.Empty;

        [JsonPropertyName("codigo_promotor")]
        public string CodigoPromotor { get; set; } = string.Empty;

        [JsonPropertyName("promotor")]
        public string Promotor { get; set; } = string.Empty;

        [JsonPropertyName("turno")]
        public string Turno { get; set; } = string.Empty;

        [JsonPropertyName("fecha")]
        public string Fecha { get; set; } = string.Empty;

        [JsonPropertyName("hora")]
        public string Hora { get; set; } = string.Empty;

        [JsonPropertyName("isla")]
        public string Isla { get; set; } = string.Empty;

        [JsonPropertyName("surtidor")]
        public string Surtidor { get; set; } = string.Empty;

        [JsonPropertyName("cara")]
        public string Cara { get; set; } = string.Empty;

        [JsonPropertyName("manguera")]
        public string Manguera { get; set; } = string.Empty;

        [JsonPropertyName("lectura_inicial")]
        public double LecturaInicial { get; set; }

        [JsonPropertyName("lectura_final")]
        public double LecturaFinal { get; set; }

        [JsonPropertyName("tanque")]
        public string Tanque { get; set; } = string.Empty;

        [JsonPropertyName("tipo_factura")]
        public string TipoFactura { get; set; } = string.Empty;

        [JsonPropertyName("prefijo")]
        public string Prefijo { get; set; } = string.Empty;

        [JsonPropertyName("consecutivo")]
        public string Consecutivo { get; set; } = string.Empty;

        [JsonPropertyName("consecutivo_anulada")]
        public string ConsecutivoAnulada { get; set; } = string.Empty;

        [JsonPropertyName("producto")]
        public string Producto { get; set; } = string.Empty;

        [JsonPropertyName("impoconsumo")]
        public double Impoconsumo { get; set; }

        [JsonPropertyName("porcentaje_iva")]
        public double PorcentajeIva { get; set; }

        [JsonPropertyName("valor_iva")]
        public double ValorIva { get; set; }

        [JsonPropertyName("precio")]
        public double Precio { get; set; }

        [JsonPropertyName("precio_diferencial")]
        public double PrecioDiferencial { get; set; }

        [JsonPropertyName("cantidad")]
        public double Cantidad { get; set; }

        [JsonPropertyName("unidad")]
        public string Unidad { get; set; } = string.Empty;

        [JsonPropertyName("descuento")]
        public double Descuento { get; set; }

        [JsonPropertyName("valor_venta")]
        public double ValorVenta { get; set; }

        [JsonPropertyName("medio_pago")]
        public string MedioPago { get; set; } = string.Empty;

        [JsonPropertyName("medio_identificacion")]
        public string MedioIdentificacion { get; set; } = string.Empty;

        [JsonPropertyName("placa")]
        public string Placa { get; set; } = string.Empty;

        [JsonPropertyName("tipo_documento_fe")]
        public string TipoDocumentoFe { get; set; } = string.Empty;

        [JsonPropertyName("numero_documento_fe")]
        public string NumeroDocumentoFe { get; set; } = string.Empty;

        [JsonPropertyName("nombre_cliente")]
        public string NombreCliente { get; set; } = string.Empty;

        [JsonPropertyName("correo")]
        public string Correo { get; set; } = string.Empty;

        [JsonPropertyName("kilometraje")]
        public string Kilometraje { get; set; } = string.Empty;

        [JsonPropertyName("telefono")]
        public string Telefono { get; set; } = string.Empty;

        [JsonPropertyName("direccion")]
        public string Direccion { get; set; } = string.Empty;

        [JsonPropertyName("tipo_negocio")]
        public string TipoNegocio { get; set; } = string.Empty;

    }

    public class VentaDetalladaResponseDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }
}
