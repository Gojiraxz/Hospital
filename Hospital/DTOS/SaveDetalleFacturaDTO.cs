using System.ComponentModel.DataAnnotations;

namespace Hospital.DTOS
{
    // DTO para crear y editar: Detalle de factura
    public class SaveDetalleFacturaDTO
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "El campo cantidad es obligatorio")]
        [Range(1, 10000, ErrorMessage = "Valor fuera de rango")]
        public int cantidad { get; set; }
        [Required(ErrorMessage = "El campo precio unitario es obligatorio")]
        [Range(0.01, 1000000, ErrorMessage = "Valor fuera de rango")]
        public decimal precioUnitario { get; set; }
        [Required]
        public int facturaId { get; set; }
        [Required]
        public int medicamentoId { get; set; }
    }
}
