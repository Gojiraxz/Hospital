namespace Hospital.Models
{
    // Entidad: Examen médico
    public class ExamenMedico
    {
        public int Id { get; set; }
        public string TipoExamen { get; set; } = string.Empty;
        public string Resultado { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;

        public int ConsultaMedicaId { get; set; }
        public virtual ConsultaMedica ConsultaMedica { get; set; } = null!;

        public int LaboratorioId { get; set; }
        public virtual Laboratorio Laboratorio { get; set; } = null!;
    }
}
