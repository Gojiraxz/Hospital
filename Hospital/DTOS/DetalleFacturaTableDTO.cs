namespace Hospital.DTOS
{
    // DTO para mostrar detalle de facturas en el DataTable
    public class DetalleFacturaTableDTO
    {
        public int id { get; set; }
        public int cantidad { get; set; }
        public decimal precioUnitario { get; set; }
        public decimal subtotal { get; set; }
        public int facturaId { get; set; }
        public string facturaName { get; set; } = string.Empty;
        public int medicamentoId { get; set; }
        public string medicamentoName { get; set; } = string.Empty;
    }
}
