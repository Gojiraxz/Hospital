namespace Hospital.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string name { get; set; } = string.Empty;
        public ICollection<Usuario> usuario { get; set; } = [];

        public ICollection<Rolepermission> Rolepermissions { get; set; } = [];
    }
}
