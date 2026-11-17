namespace Hospital.Models
{
    // Entidad: Habitación
    public class Habitacion
    {
        public int Id { get; set; }
        public string Numero { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;

        // Paciente que ocupa la habitacion (null si esta libre)
        public int? PacienteId { get; set; }
        public virtual Paciente? Paciente { get; set; }

        public ICollection<Cirugia> Cirugias { get; set; } = [];
    }
}
