namespace Hospital.Models
{
    public class Cirugia
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string Estado { get; set; } = string.Empty;

        public int DoctorId { get; set; }
        public virtual Doctor Doctor { get; set; } = null!;

        public int HabitacionId { get; set; }
        public virtual Habitacion Habitacion { get; set; } = null!;

        public int PacienteId { get; set; }
        public virtual Paciente Paciente { get; set; } = null!;
    }
}
