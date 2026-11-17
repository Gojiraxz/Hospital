namespace Hospital.Models
{
    // Entidad: Factura
    public class Factura
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; } = string.Empty;

        public int PacienteId { get; set; }
        public virtual Paciente Paciente { get; set; } = null!;

        public ICollection<DetalleFactura> DetallesFactura { get; set; } = [];
    }
}
