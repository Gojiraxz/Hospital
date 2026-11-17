namespace Hospital.Models
{
    // Entidad: Laboratorio
    public class Laboratorio
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Ubicacion { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;

        public ICollection<ExamenMedico> ExamenesMedicos { get; set; } = [];
    }
}
