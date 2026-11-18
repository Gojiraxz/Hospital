namespace Hospital.DTOS
{
    // DTO para mostrar consultas médicas en el DataTable
    public class ConsultaMedicaTableDTO
    {
        public int id { get; set; }
        public DateTime fecha { get; set; }
        public string diagnostico { get; set; } = string.Empty;
        public string tratamiento { get; set; } = string.Empty;
        public int doctorId { get; set; }
        public string doctorName { get; set; } = string.Empty;
        public int citaId { get; set; }
        public string citaName { get; set; } = string.Empty;
        public int pacienteId { get; set; }
        public string pacienteName { get; set; } = string.Empty;
        public string display { get; set; } = string.Empty;
    }
}
