using Hospital.DTOS;

namespace Hospital.Service
{
    public interface IAuthService
    {
        Task<(bool Success, string? ErrorMessage)> RegisterAsync(RegisterRequestDto dto);
        Task<(bool Success, LoginResponseDTO? result, String? Errormessage)> LoginAsync(LoginRequestDto dto);
        Task<bool> ConfirmEmailAsync(string email, string token);
        Task<bool> SendResetPassswordLinkAsync(string email);
        Task<bool> ResetPasswordAsync(ResetPasswordRequestDTO dTO);
        Task<bool> ResendConfirmationEmailAsync(string email);
        Task<bool> ChangePassWordAsync(string userName, ChangePasswordDTO dTO);
        Task<LoginResponseDTO?> RefreshAcessTokenAsync(string refreshToken);
        Task<bool> AdminExistsAsync();
        Task<(bool Success, string? ErrorMessage)> SetupAdminAsync(SetupAdminDTO dto);
        Task<(bool Success, LoginResponseDTO? result, string? ErrorMessage)> LoginPacienteAsync(PacienteLoginDTO dto);
    }
}
