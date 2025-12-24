using System;

namespace Domain.Entities
{
    // Modelo actualizado con TODAS las columnas del CSV real de Terpel (39 campos)
    public class RegistroVentaEDS
    {
        // Campos básicos de la venta (28 campos originales)
        public required string CódigoEstación { get; set; }
        public required string Estación { get; set; }
        public required string CódigoPromotor { get; set; }
        public required string Promotor { get; set; }
        public required string Turno { get; set; }
        public required string Fecha { get; set; }
        public required string Hora { get; set; }
        public required string Isla { get; set; }
        public required string Surtidor { get; set; }
        public required string Cara { get; set; }
        public required string Manguera { get; set; }
        public required decimal LecturaInicial { get; set; }
        public required decimal LecturaFinal { get; set; }
        public required string Tanque { get; set; }
        public required string TipoFactura { get; set; }
        public required string Prefijo { get; set; }
        public required string Consecutivo { get; set; }
        public required string Referencia { get; set; }
        public required string Producto { get; set; }
        public required decimal Impoconsumo { get; set; }
        public required decimal IvaPorcentaje { get; set; }
        public required decimal ValorIva { get; set; }
        public required decimal Precio { get; set; }
        public required decimal PrecioDiferencial { get; set; }
        public required decimal Cantidad { get; set; }
        public required string Unidad { get; set; }
        public required decimal Descuento { get; set; }
        public required decimal ValorVenta { get; set; }

        // Campos adicionales del CSV real (11 campos nuevos)
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
