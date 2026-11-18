namespace Hospital.DTOS
{
    // DTO para mostrar medicamentos en el DataTable
    public class MedicamentoTableDTO
    {
        public int id { get; set; }
        public string nombre { get; set; } = string.Empty;
        public string descripcion { get; set; } = string.Empty;
        public int stock { get; set; }
        public decimal precio { get; set; }
        public int proveedorId { get; set; }
        public string proveedorName { get; set; } = string.Empty;
        public string display { get; set; } = string.Empty;
    }
}
