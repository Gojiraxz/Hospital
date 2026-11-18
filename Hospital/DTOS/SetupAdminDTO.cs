using System.ComponentModel.DataAnnotations;

namespace Hospital.DTOS
{
    // Creacion de la primera cuenta de administrador
    public class SetupAdminDTO
    {
        [Required, MinLength(3), MaxLength(45)]
        public string username { get; set; } = string.Empty;
        [Required, EmailAddress, MaxLength(100)]
        public string email { get; set; } = string.Empty;
        [Required, MinLength(8)]
        public string password { get; set; } = string.Empty;
    }
}
