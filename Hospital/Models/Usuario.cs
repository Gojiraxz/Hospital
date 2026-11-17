namespace Hospital.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombreusuario { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;

        public string email { get; set; } = string.Empty;

        public bool emailconfirmed { get; set; }
        public string? emailconfirmationtoken { get; set; }
        public string? refreshtoken { get; set; }
        public DateTime? refreshtokenexoirytime { get; set; }

        public string? resetpasswordtoken { get; set; }
        public DateTime? PasswordResetTokenexpirytime { get; set; }

        public int RolId { get; set; }
        public virtual Role Role { get; set; } = null!;
        public ICollection<UsuarioPermiso> UsuarioPermisos { get; set; } = [];
    }
}
