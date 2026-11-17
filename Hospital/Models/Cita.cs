


namespace Hospital.Models
{
    // Entidad: Cita
    public class Cita
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string Motivo { get; set; } = string.Empty;

        public int DoctorId { get; set; }
        public virtual Doctor Doctor { get; set; } = null!;

        public int PacienteId { get; set; }
        public virtual Paciente Paciente { get; set; } = null!;

        public ICollection<ConsultaMedica> ConsultasMedicas { get; set; } = [];
    }
}
