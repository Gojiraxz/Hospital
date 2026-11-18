using System.ComponentModel.DataAnnotations;

namespace Hospital.DTOS
{
    // DTO para crear y editar: Proveedor
    public class SaveProveedorDTO
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "El campo nombre es obligatorio")]
        [MaxLength(45)]
        public string nombre { get; set; } = string.Empty;
        [Required(ErrorMessage = "El campo correo es obligatorio")]
        [MaxLength(100)]
        [EmailAddress]
        public string correo { get; set; } = string.Empty;
        [Required(ErrorMessage = "El campo registro sanitario (cofepris) es obligatorio")]
        [MaxLength(45)]
        public string cofepris { get; set; } = string.Empty;
    }
}
