namespace Hospital.DTOS
{
    // DTO para mostrar especialidades en el DataTable
    public class EspecialidadTableDTO
    {
        public int id { get; set; }
        public string nombre { get; set; } = string.Empty;
        public string display { get; set; } = string.Empty;
    }
}
