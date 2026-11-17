namespace Hospital.Models
{
    public class UsuarioPermiso
    {
        public int Usuarioid { get; set; }
        public virtual Usuario Usuario { get; set; } = null!;
        public int PermissionId { get; set; }
        public virtual permission Permission { get; set; } = null!;
        public bool isGranted { get; set; }
    }
}
