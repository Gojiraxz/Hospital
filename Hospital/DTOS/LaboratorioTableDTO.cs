namespace Hospital.DTOS
{
    // DTO para mostrar laboratorios en el DataTable
    public class LaboratorioTableDTO
    {
        public int id { get; set; }
        public string nombre { get; set; } = string.Empty;
        public string ubicacion { get; set; } = string.Empty;
        public string telefono { get; set; } = string.Empty;
        public string display { get; set; } = string.Empty;
    }
}
