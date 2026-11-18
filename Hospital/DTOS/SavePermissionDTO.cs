using System.ComponentModel.DataAnnotations;

namespace Hospital.DTOS
{
    public class SavePermissionDTO
    {
        public int? id { get; set; }
        [Required]
        [MaxLength(100)]
        [RegularExpression(@"^[a-zA-Z0-9_]+\.[a-zA-Z0-9_]+$", ErrorMessage = "Use el formato recurso.accion, por ejemplo: paciente.ver")]
        public string name { get; set; } = string.Empty;
        [MaxLength(250)]
        public string? Descripcion { get; set; }
    }
}
