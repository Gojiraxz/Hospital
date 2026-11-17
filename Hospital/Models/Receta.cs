namespace Hospital.Models
{
    // Entidad: Receta (solicitud de receta hecha por un paciente a un doctor)
    public class Receta
    {
        public int Id { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public string Medicamento { get; set; } = string.Empty;
        public string Indicaciones { get; set; } = string.Empty;
        // Pendiente, Aprobada o Rechazada
        public string Estado { get; set; } = "Pendiente";
        // Respuesta o indicaciones del doctor/admin al paciente
        public string? Respuesta { get; set; }

        public int DoctorId { get; set; }
        public virtual Doctor Doctor { get; set; } = null!;

        public int PacienteId { get; set; }
        public virtual Paciente Paciente { get; set; } = null!;
    }
}
