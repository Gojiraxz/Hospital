using System.ComponentModel.DataAnnotations;

namespace Hospital.DTOS
{
    // DTO para crear y editar: Turno
    public class SaveTurnoDTO
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "El campo hora de inicio es obligatorio")]
        [MaxLength(45)]
        [RegularExpression(@"^([01]\d|2[0-3]):[0-5]\d$", ErrorMessage = "Formato de hora invalido (HH:mm)")]
        public string inicio { get; set; } = string.Empty;
        [Required(ErrorMessage = "El campo hora final es obligatorio")]
        [MaxLength(45)]
        [RegularExpression(@"^([01]\d|2[0-3]):[0-5]\d$", ErrorMessage = "Formato de hora invalido (HH:mm)")]
        public string final { get; set; } = string.Empty;
        [Required]
        public int enfermeroId { get; set; }
        [Required]
        public int doctorId { get; set; }
    }
}
