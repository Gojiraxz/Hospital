namespace Hospital.Models
{
    public class permission
    {
        public int Id { get; set; }
        public string name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public ICollection<UsuarioPermiso> Permissions { get; set; } = [];
        public ICollection<Rolepermission> rolepermissions { get; set; } = [];
    }
}
