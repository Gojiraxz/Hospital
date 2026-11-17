namespace Hospital.Models
{
    // Entidad: Detalle de factura
    public class DetalleFactura
    {
        public int Id { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }

        public int FacturaId { get; set; }
        public virtual Factura Factura { get; set; } = null!;

        public int MedicamentoId { get; set; }
        public virtual Medicamento Medicamento { get; set; } = null!;
    }
}
