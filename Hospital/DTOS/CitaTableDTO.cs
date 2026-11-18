namespace Hospital.DTOS
{
    // DTO para mostrar citas en el DataTable
    public class CitaTableDTO
    {
        public int id { get; set; }
        public DateTime fecha { get; set; }
        public string estado { get; set; } = string.Empty;
        public string motivo { get; set; } = string.Empty;
        public int doctorId { get; set; }
        public string doctorName { get; set; } = string.Empty;
        public int pacienteId { get; set; }
        public string pacienteName { get; set; } = string.Empty;
        public string display { get; set; } = string.Empty;
    }
}
