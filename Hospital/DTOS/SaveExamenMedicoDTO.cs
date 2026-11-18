using System.ComponentModel.DataAnnotations;

namespace Hospital.DTOS
{
    // DTO para crear y editar: Examen médico
    public class SaveExamenMedicoDTO
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "El campo tipo de examen es obligatorio")]
        [MaxLength(45)]
        public string tipoExamen { get; set; } = string.Empty;
        [Required(ErrorMessage = "El campo resultado es obligatorio")]
        [MaxLength(250)]
        public string resultado { get; set; } = string.Empty;
        [Required(ErrorMessage = "El campo estado es obligatorio")]
        [MaxLength(45)]
        public string estado { get; set; } = string.Empty;
        [Required]
        public int consultaMedicaId { get; set; }
        [Required]
        public int laboratorioId { get; set; }
    }
}
