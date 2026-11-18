using System.ComponentModel.DataAnnotations;

namespace Hospital.DTOS
{
    public class CreateRolDTO
    {
        [Required(ErrorMessage = "el nombre es requerido")]
        [MaxLength(100)]
        public string name { get; set; } = string.Empty;
    }
}
