namespace Hospital.Models
{
    // Entidad: Especialidad
    public class Especialidad
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;

        public ICollection<Doctor> Doctores { get; set; } = [];
        public ICollection<Enfermero> Enfermeros { get; set; } = [];
    }
}
