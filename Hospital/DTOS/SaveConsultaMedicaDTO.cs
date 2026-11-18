using System.ComponentModel.DataAnnotations;

namespace Hospital.DTOS
{
    // DTO para crear y editar: Consulta médica
    public class SaveConsultaMedicaDTO
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "El campo fecha y hora es obligatorio")]
        public DateTime fecha { get; set; }
        [Required(ErrorMessage = "El campo diagnóstico es obligatorio")]
        [MaxLength(250)]
        public string diagnostico { get; set; } = string.Empty;
        [Required(ErrorMessage = "El campo tratamiento es obligatorio")]
        [MaxLength(250)]
        public string tratamiento { get; set; } = string.Empty;
        [Required]
        public int doctorId { get; set; }
        [Required]
        public int citaId { get; set; }
        [Required]
        public int pacienteId { get; set; }
    }
}
