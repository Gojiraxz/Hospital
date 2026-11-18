namespace Hospital.DTOS
{
    public class ResetPasswordRequestDTO
    {
        public string email { get; set; } = null!;
        public string token { get; set; } = null!;
        public string newPassword { get; set; } = null!;
    }
}
