namespace Hospital.Models
{
    // Entidad: Consulta médica
    public class ConsultaMedica
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Diagnostico { get; set; } = string.Empty;
        public string Tratamiento { get; set; } = string.Empty;

        public int DoctorId { get; set; }
        public virtual Doctor Doctor { get; set; } = null!;

        public int CitaId { get; set; }
        public virtual Cita Cita { get; set; } = null!;

        public int PacienteId { get; set; }
        public virtual Paciente Paciente { get; set; } = null!;

        public ICollection<ExamenMedico> ExamenesMedicos { get; set; } = [];
    }
}
