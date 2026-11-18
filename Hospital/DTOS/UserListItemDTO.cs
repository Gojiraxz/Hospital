namespace Hospital.DTOS
{
    public class UserListItemDTO
    {
        public int Id { get; set; }
        public string username { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
    }
}
