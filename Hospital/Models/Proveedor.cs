namespace Hospital.Models
{
    // Entidad: Proveedor
    public class Proveedor
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Cofepris { get; set; } = string.Empty;

        public ICollection<Medicamento> Medicamentos { get; set; } = [];
    }
}
