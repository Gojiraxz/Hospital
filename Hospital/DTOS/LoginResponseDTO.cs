namespace Hospital.DTOS
{
    public class LoginResponseDTO
    {
        public string accessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public DateTime? RefreshTokenExpiresAt { get; set; }
    }
}
