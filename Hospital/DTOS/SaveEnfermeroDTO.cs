using System.ComponentModel.DataAnnotations;

namespace Hospital.DTOS
{
    // DTO para crear y editar: Enfermero
    public class SaveEnfermeroDTO
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "El campo nombre es obligatorio")]
        [MaxLength(45)]
        public string nombre { get; set; } = string.Empty;
        [Required(ErrorMessage = "El campo apellido es obligatorio")]
        [MaxLength(45)]
        public string apellido { get; set; } = string.Empty;
        [Required]
        public int especialidadId { get; set; }
    }
}
