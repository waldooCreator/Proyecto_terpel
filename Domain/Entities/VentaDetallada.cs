namespace Domain.Entities
{
    public class VentaDetallada
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public int ArchivoId { get; set; }
        public int EdsId { get; set; }
        public string? Estacion { get; set; }
        public string? CodigoEstacion { get; set; }
        public string? CodigoPromotor { get; set; }
        public string? Promotor { get; set; }
        public string? Turno { get; set; }
        public string? Fecha { get; set; }
        public string? Hora { get; set; }
        public string? Isla { get; set; }
        public string? Surtidor { get; set; }
        public string? Cara { get; set; }
        public string? Manguera { get; set; }
        public double LecturaInicial { get; set; }
        public double LecturaFinal { get; set; }
        public string? Tanque { get; set; }
        public string? TipoFactura { get; set; }
        public string? Prefijo { get; set; }
        public string? Consecutivo { get; set; }
        public string? ConsecutivoAnulada { get; set; }
        public string? Producto { get; set; }
        public double Impoconsumo { get; set; }
        public double PorcentajeIva { get; set; }
        public double ValorIva { get; set; }
        public double Precio { get; set; }
        public double PrecioDiferencial { get; set; }
        public double Cantidad { get; set; }
        public string? Unidad { get; set; }
        public double Descuento { get; set; }
        public double ValorVenta { get; set; }
        public string? MedioPago { get; set; }
        public string? MedioIdentificacion { get; set; }
        public string? Placa { get; set; }
        public string? TipoDocumentoFe { get; set; }
        public string? NumeroDocumentoFe { get; set; }
        public string? NombreCliente { get; set; }
        public string? Correo { get; set; }
        public string? Kilometraje { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public string? TipoNegocio { get; set; }
        public string? CreatedAt { get; set; }
    }
}
