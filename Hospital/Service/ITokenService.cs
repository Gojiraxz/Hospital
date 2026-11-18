using Hospital.Models;

namespace Hospital.Service
{
    public interface ITokenService
    {
        string GenerateAccessToken(Usuario usuario);
        string GenerateRefreshToken();
        string GeneratePacienteToken(Paciente paciente, int minutes);
        string GeneratePasswordResetToken();

    }
}
