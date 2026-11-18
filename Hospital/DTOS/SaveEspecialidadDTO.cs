using System.ComponentModel.DataAnnotations;

namespace Hospital.DTOS
{
    // DTO para crear y editar: Especialidad
    public class SaveEspecialidadDTO
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "El campo nombre es obligatorio")]
        [MaxLength(45)]
        public string nombre { get; set; } = string.Empty;
    }
}
