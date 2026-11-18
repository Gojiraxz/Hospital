namespace Hospital.DTOS
{
    // DTO para mostrar facturas en el DataTable
    public class FacturaTableDTO
    {
        public int id { get; set; }
        public DateTime fecha { get; set; }
        public decimal total { get; set; }
        public string estado { get; set; } = string.Empty;
        public int pacienteId { get; set; }
        public string pacienteName { get; set; } = string.Empty;
        public string display { get; set; } = string.Empty;
    }
}
