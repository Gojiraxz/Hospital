namespace Hospital.DTOS
{
    // DTO para mostrar cirugías en el DataTable
    public class CirugiaTableDTO
    {
        public int id { get; set; }
        public string nombre { get; set; } = string.Empty;
        public DateTime fecha { get; set; }
        public string estado { get; set; } = string.Empty;
        public int doctorId { get; set; }
        public string doctorName { get; set; } = string.Empty;
        public int habitacionId { get; set; }
        public string habitacionName { get; set; } = string.Empty;
        public int pacienteId { get; set; }
        public string pacienteName { get; set; } = string.Empty;
    }
}
