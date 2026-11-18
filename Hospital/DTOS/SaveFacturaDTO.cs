using System.ComponentModel.DataAnnotations;

namespace Hospital.DTOS
{
    // DTO para crear y editar: Factura
    public class SaveFacturaDTO
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "El campo fecha es obligatorio")]
        public DateTime fecha { get; set; }
        [Required(ErrorMessage = "El campo estado es obligatorio")]
        [MaxLength(45)]
        public string estado { get; set; } = string.Empty;
        [Required]
        public int pacienteId { get; set; }
    }
}
