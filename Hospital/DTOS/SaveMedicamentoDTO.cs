using System.ComponentModel.DataAnnotations;

namespace Hospital.DTOS
{
    // DTO para crear y editar: Medicamento
    public class SaveMedicamentoDTO
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "El campo nombre es obligatorio")]
        [MaxLength(45)]
        public string nombre { get; set; } = string.Empty;
        [Required(ErrorMessage = "El campo descripción es obligatorio")]
        [MaxLength(250)]
        public string descripcion { get; set; } = string.Empty;
        [Required(ErrorMessage = "El campo stock es obligatorio")]
        [Range(0, 100000, ErrorMessage = "Valor fuera de rango")]
        public int stock { get; set; }
        [Required(ErrorMessage = "El campo precio es obligatorio")]
        [Range(0.01, 1000000, ErrorMessage = "Valor fuera de rango")]
        public decimal precio { get; set; }
        [Required]
        public int proveedorId { get; set; }
    }
}
