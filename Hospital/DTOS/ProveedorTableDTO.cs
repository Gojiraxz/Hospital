namespace Hospital.DTOS
{
    // DTO para mostrar proveedores en el DataTable
    public class ProveedorTableDTO
    {
        public int id { get; set; }
        public string nombre { get; set; } = string.Empty;
        public string correo { get; set; } = string.Empty;
        public string cofepris { get; set; } = string.Empty;
        public string display { get; set; } = string.Empty;
    }
}
