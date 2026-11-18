using System.ComponentModel.DataAnnotations;

namespace Hospital.DTOS
{
    // DTO para crear y editar: Habitación
    public class SaveHabitacionDTO
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "El campo número es obligatorio")]
        [MaxLength(45)]
        public string numero { get; set; } = string.Empty;
        [Required(ErrorMessage = "El campo estado es obligatorio")]
        [MaxLength(45)]
        public string estado { get; set; } = string.Empty;
        // Paciente asignado (opcional). Si se asigna, la habitacion queda Ocupada.
        public int? pacienteId { get; set; }
    }
}
