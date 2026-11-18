using System.ComponentModel.DataAnnotations;

namespace Hospital.DTOS
{
    // DTO para crear y editar: Paciente
    public class SavePacienteDTO
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "El campo nombre es obligatorio")]
        [MaxLength(45)]
        public string nombre { get; set; } = string.Empty;
        [Required(ErrorMessage = "El campo apellido es obligatorio")]
        [MaxLength(45)]
        public string apellido { get; set; } = string.Empty;
        [Required(ErrorMessage = "El campo cédula es obligatorio")]
        [RegularExpression("^[0-9A-Za-z]{14}$", ErrorMessage = "La cédula debe tener exactamente 14 caracteres, sin guiones")]
        public string cedula { get; set; } = string.Empty;
        [Required(ErrorMessage = "El campo fecha de nacimiento es obligatorio")]
        public DateTime fechaNacimiento { get; set; }
        [Required(ErrorMessage = "El campo sexo es obligatorio")]
        [MaxLength(45)]
        public string sexo { get; set; } = string.Empty;
        [Required(ErrorMessage = "El campo teléfono es obligatorio")]
        [RegularExpression("^[0-9]{8}$", ErrorMessage = "El teléfono debe tener exactamente 8 dígitos")]
        public string telefono { get; set; } = string.Empty;
        [Required(ErrorMessage = "El campo dirección es obligatorio")]
        [MaxLength(150)]
        public string direccion { get; set; } = string.Empty;
    }
}
