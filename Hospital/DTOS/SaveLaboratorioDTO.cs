using System.ComponentModel.DataAnnotations;

namespace Hospital.DTOS
{
    // DTO para crear y editar: Laboratorio
    public class SaveLaboratorioDTO
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "El campo nombre es obligatorio")]
        [MaxLength(45)]
        public string nombre { get; set; } = string.Empty;
        [Required(ErrorMessage = "El campo ubicación es obligatorio")]
        [MaxLength(45)]
        public string ubicacion { get; set; } = string.Empty;
        [Required(ErrorMessage = "El campo teléfono es obligatorio")]
        [MaxLength(15)]
        public string telefono { get; set; } = string.Empty;
    }
}
