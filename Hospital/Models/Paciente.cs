namespace Hospital.Models
{
    // Entidad: Paciente
    public class Paciente
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Cedula { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public string Sexo { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;

        public ICollection<Cita> Citas { get; set; } = [];
        public ICollection<ConsultaMedica> ConsultasMedicas { get; set; } = [];
        public ICollection<Cirugia> Cirugias { get; set; } = [];
        public ICollection<Factura> Facturas { get; set; } = [];
        public ICollection<Receta> Recetas { get; set; } = [];
        public ICollection<Habitacion> Habitaciones { get; set; } = [];
    }
}
