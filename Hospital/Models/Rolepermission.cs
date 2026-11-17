namespace Hospital.Models
{
    public class Rolepermission
    {
        public int RoleId { get; set; }
        public virtual Role Role { get; set; }= null!;
        public int PermissionId { get; set; }
        public virtual permission Permission { get; set; } = null!;
    }
}
