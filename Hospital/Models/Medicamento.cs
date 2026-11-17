namespace Hospital.Models
{
    // Entidad: Medicamento
    public class Medicamento
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public int Stock { get; set; }
        public decimal Precio { get; set; }

        public int ProveedorId { get; set; }
        public virtual Proveedor Proveedor { get; set; } = null!;

        public ICollection<DetalleFactura> DetallesFactura { get; set; } = [];
    }
}
