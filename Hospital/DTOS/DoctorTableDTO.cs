namespace Hospital.DTOS
{
    // DTO para mostrar doctores en el DataTable
    public class DoctorTableDTO
    {
        public int id { get; set; }
        public string nombre { get; set; } = string.Empty;
        public string apellido { get; set; } = string.Empty;
        public int especialidadId { get; set; }
        public string especialidadName { get; set; } = string.Empty;
        public string display { get; set; } = string.Empty;
    }
}
