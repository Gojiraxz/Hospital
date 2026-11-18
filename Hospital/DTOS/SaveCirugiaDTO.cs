using System.ComponentModel.DataAnnotations;

namespace Hospital.DTOS
{
    // DTO para crear y editar: Cirugía
    public class SaveCirugiaDTO
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "El campo nombre es obligatorio")]
        [MaxLength(45)]
        public string nombre { get; set; } = string.Empty;
        [Required(ErrorMessage = "El campo fecha y hora es obligatorio")]
        public DateTime fecha { get; set; }
        [Required(ErrorMessage = "El campo estado es obligatorio")]
        [MaxLength(45)]
        public string estado { get; set; } = string.Empty;
        [Required]
        public int doctorId { get; set; }
        [Required]
        public int habitacionId { get; set; }
        [Required]
        public int pacienteId { get; set; }
    }
}
