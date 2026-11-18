namespace Hospital.DTOS
{
    public class ChangePasswordDTO
    {
        public required string currentPassword { get; set; }
        public required string newPassword { get; set; }
    }
}
