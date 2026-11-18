namespace Hospital.DTOS
{
    public class RegisterRequestDto
    {
        public required string username { get; set; }
        public required string email { get; set; }
        public required string password { get; set; }
        public int roleId { get; set; } = 2;
    }
}
