namespace Hospital.Models
{
    // Entidad: Turno
    public class Turno
    {
        public int Id { get; set; }
        public string Inicio { get; set; } = string.Empty;
        public string Final { get; set; } = string.Empty;

        public int EnfermeroId { get; set; }
        public virtual Enfermero Enfermero { get; set; } = null!;

        public int DoctorId { get; set; }
        public virtual Doctor Doctor { get; set; } = null!;
    }
}
