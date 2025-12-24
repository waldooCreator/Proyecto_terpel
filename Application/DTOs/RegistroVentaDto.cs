namespace Application.DTOs
{
    public class RegistroVentaDto
    {
        // 28 campos originales
        public string CódigoEstación { get; set; } = string.Empty;
        public string Estación { get; set; } = string.Empty;
        public string CódigoPromotor { get; set; } = string.Empty;
        public string Promotor { get; set; } = string.Empty;
        public string Turno { get; set; } = string.Empty;
        public string Fecha { get; set; } = string.Empty;
        public string Hora { get; set; } = string.Empty;
        public string Isla { get; set; } = string.Empty;
        public string Surtidor { get; set; } = string.Empty;
        public string Cara { get; set; } = string.Empty;
        public string Manguera { get; set; } = string.Empty;
        public string LecturaInicial { get; set; } = string.Empty;
        public string LecturaFinal { get; set; } = string.Empty;
        public string Tanque { get; set; } = string.Empty;
        public string TipoFactura { get; set; } = string.Empty;
        public string Prefijo { get; set; } = string.Empty;
        public string Consecutivo { get; set; } = string.Empty;
        public string Referencia { get; set; } = string.Empty;
        public string Producto { get; set; } = string.Empty;
        public string Impoconsumo { get; set; } = string.Empty;
        public string IvaPorcentaje { get; set; } = string.Empty;
        public string ValorIva { get; set; } = string.Empty;
        public string Precio { get; set; } = string.Empty;
        public string PrecioDiferencial { get; set; } = string.Empty;
        public string Cantidad { get; set; } = string.Empty;
        public string Unidad { get; set; } = string.Empty;
        public string Descuento { get; set; } = string.Empty;
        public string ValorVenta { get; set; } = string.Empty;

        // 11 campos adicionales del CSV real
        public string? MedioPago { get; set; }
        public string? MedioIdentificacion { get; set; }
        public string? Placa { get; set; }
        public string? TipoDocumentoFE { get; set; }
        public string? NumeroDocumentoFE { get; set; }
        public string? NombreCliente { get; set; }
        public string? Correo { get; set; }
        public string? Kilometraje { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public string? TipoNegocio { get; set; }
    }
}
