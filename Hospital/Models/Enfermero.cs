namespace Hospital.Models
{
    public class Enfermero
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;

        public int EspecialidadId { get; set; }
        public virtual Especialidad Especialidad { get; set; } = null!;

        public ICollection<Turno> Turnos { get; set; } = [];
    }
}
