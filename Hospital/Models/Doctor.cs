namespace Hospital.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;

        public int EspecialidadId { get; set; }
        public virtual Especialidad Especialidad { get; set; } = null!;

        public ICollection<Turno> Turnos { get; set; } = [];
        public ICollection<Cita> Citas { get; set; } = [];
        public ICollection<ConsultaMedica> ConsultasMedicas { get; set; } = [];
        public ICollection<Cirugia> Cirugias { get; set; } = [];
        public ICollection<Receta> Recetas { get; set; } = [];
    }
}
