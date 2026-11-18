using System.ComponentModel.DataAnnotations;

namespace Hospital.DTOS
{
    public class CreatePermissionDTO
    {
        [Required(ErrorMessage = "el nombre es requerido")]
        [MaxLength(150)]
        [RegularExpression(@"^[a-zA-Z0-9_]+\.[a-zA-Z0-9_]+$", ErrorMessage = "Use el formato recurso.accion, por ejemplo: paciente.ver")]
        public string name { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
    }
}
