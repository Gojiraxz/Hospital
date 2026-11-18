namespace Hospital.DTOS
{
    // DTO para mostrar turnos en el DataTable
    public class TurnoTableDTO
    {
        public int id { get; set; }
        public string inicio { get; set; } = string.Empty;
        public string final { get; set; } = string.Empty;
        public int enfermeroId { get; set; }
        public string enfermeroName { get; set; } = string.Empty;
        public int doctorId { get; set; }
        public string doctorName { get; set; } = string.Empty;
    }
}
