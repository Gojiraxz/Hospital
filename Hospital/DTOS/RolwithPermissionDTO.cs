namespace Hospital.DTOS
{
    public class RolwithPermissionDTO
    {
        public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public List<PermissionDTO> Permissions { get; set; } =[];
    }
}
