using System.ComponentModel.DataAnnotations;

namespace Hospital.DTOS
{
    public class CreateUserDTO
    {
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        [MinLength(3, ErrorMessage = "El nombre de usuario debe tener minimo 3 caracteres")]
        [MaxLength(100, ErrorMessage = "El nombre de usuario es demasiado largo")]
        public string username { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es requerido")]
        [EmailAddress(ErrorMessage = "Ingresa un correo valido")]
        [MaxLength(200, ErrorMessage = "El correo es demasiado largo")]
        public string email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contrasena es requerida")]
        [MinLength(8, ErrorMessage = "La contrasena debe tener minimo 8 caracteres")]
        [MaxLength(100, ErrorMessage = "La contrasena es demasiado larga")]
        public string password { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Selecciona un rol")]
        public int roleId { get; set; }

        // Permisos extra que se conceden directamente al usuario (ademas de los de su rol). Es opcional.
        public List<int>? permissionIds { get; set; }
    }
}
