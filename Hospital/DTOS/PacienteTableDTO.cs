namespace Hospital.DTOS
{
    // DTO para mostrar pacientes en el DataTable
    public class PacienteTableDTO
    {
        public int id { get; set; }
        public string nombre { get; set; } = string.Empty;
        public string apellido { get; set; } = string.Empty;
        public string cedula { get; set; } = string.Empty;
        public DateTime fechaNacimiento { get; set; }
        public string sexo { get; set; } = string.Empty;
        public string telefono { get; set; } = string.Empty;
        public string direccion { get; set; } = string.Empty;
        public string display { get; set; } = string.Empty;
    }
}
