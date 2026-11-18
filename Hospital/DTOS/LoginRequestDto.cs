namespace Hospital.DTOS
{
    // Inicio de sesion del personal (admin/doctor): nombre de usuario + contrasena
    public class LoginRequestDto
    {
        public string username { get; set; } = null!;
        public string password { get; set; } = null!;
    }
}
