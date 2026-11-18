namespace Hospital.DTOS
{
    // DTO para mostrar habitaciones en el DataTable
    public class HabitacionTableDTO
    {
        public int id { get; set; }
        public string numero { get; set; } = string.Empty;
        public string estado { get; set; } = string.Empty;
        public int? pacienteId { get; set; }
        public string pacienteName { get; set; } = string.Empty;
        public string display { get; set; } = string.Empty;
    }
}
