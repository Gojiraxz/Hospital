using AutoMapper;
using System.Security.Cryptography;
using Hospital.DTOS;
using Hospital.Models;
using Hospital.Repository;
using Hospital.Models.settings;
using Microsoft.Extensions.Options;

namespace Hospital.Service
{
    public class AuthService(IUsuarioRepository userRepo, IRoleRepository roleRepo, ITokenService tokenService,  IEmailService emailService, IMapper mapper, IOptions<smtpSettings> smtpOptions, IPacienteRepository pacienteRepo) : IAuthService
    {
        private readonly IUsuarioRepository _userRepo = userRepo;
        private readonly IRoleRepository _roleRepo = roleRepo;
        private readonly ITokenService _tokenService = tokenService;
        private readonly IEmailService _emailService = emailService;
        private readonly IMapper _mapper = mapper;
        private readonly smtpSettings _smtp = smtpOptions.Value;
        private readonly IPacienteRepository _pacienteRepo = pacienteRepo;
        // El paciente entra con cedula y su sesion dura 60 minutos
        private const int PacienteSessionMinutes = 60;

        public async Task<bool> ChangePassWordAsync(string userName, ChangePasswordDTO dTO)
        {
           var user= await _userRepo.GetUsuarioByName(userName);
            if (user == null || !_userRepo.ValidatePassword(user, dTO.currentPassword))
                return false;
            user.password = BCrypt.Net.BCrypt.EnhancedHashPassword(dTO.newPassword);
            await _userRepo.SaveAsync(user);
            return true;
        }

        public async Task<bool> ConfirmEmailAsync(string email, string token)
        {
           var user= await _userRepo.GetUsuarioByEmail(email);
            if (user == null || user.emailconfirmationtoken != token)
                return false;
            var decoded=Uri.UnescapeDataString(token.Trim());
            if (!string.Equals(user.emailconfirmationtoken, decoded, StringComparison.OrdinalIgnoreCase))
                return false;
            user.emailconfirmed = true;
            user.emailconfirmationtoken = null;
            await _userRepo.SaveAsync(user);
            return true;
        }

        public async Task<(bool Success, LoginResponseDTO? result, string? Errormessage)> LoginAsync(LoginRequestDto dto)
        {
            var username = (dto.username ?? string.Empty).Trim();
            var user = string.IsNullOrEmpty(username) ? null : await _userRepo.GetUsuarioByName(username);
            if (user == null || !_userRepo.ValidatePassword(user, dto.password ?? string.Empty))
                return (false, null, "Usuario o contraseña incorrectos");

            if (!user.emailconfirmed)
                return (false, null, "La cuenta aun no ha sido confirmada");
            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();
            
            user.refreshtoken = refreshToken;
            user.refreshtokenexoirytime= DateTime.Now.AddDays(7);
            await _userRepo.SaveAsync(user);
            var result = new LoginResponseDTO
            {
                accessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.Now.AddMinutes(15),
                RefreshTokenExpiresAt = user.refreshtokenexoirytime
            };
            return (true, result, null);
        }

        public Task<bool> AdminExistsAsync() => _userRepo.AnyAdminAsync();

        // Crea la primera cuenta de administrador. Solo funciona si todavia no existe ninguna.
        public async Task<(bool Success, string? ErrorMessage)> SetupAdminAsync(SetupAdminDTO dto)
        {
            if (await _userRepo.AnyAdminAsync())
                return (false, "Ya existe una cuenta de administrador");
            var adminRole = await _roleRepo.GetByNameAsync("Admin");
            if (adminRole == null)
                return (false, "No se encontro el rol Admin. Reinicia la aplicacion para crear los datos iniciales.");
            var username = dto.username.Trim();
            if (await _userRepo.GetUsuarioByName(username) != null)
                return (false, "Ese nombre de usuario ya existe");

            await _userRepo.CreateAsync(new Usuario
            {
                Nombreusuario = username,
                email = dto.email.Trim(),
                password = BCrypt.Net.BCrypt.EnhancedHashPassword(dto.password),
                emailconfirmed = true,
                RolId = adminRole.Id
            });
            return (true, null);
        }

        // Inicio de sesion del paciente por cedula (solo si el admin ya lo registro)
        public async Task<(bool Success, LoginResponseDTO? result, string? ErrorMessage)> LoginPacienteAsync(PacienteLoginDTO dto)
        {
            var paciente = await _pacienteRepo.GetByCedulaAsync(dto.cedula);
            if (paciente == null)
                return (false, null, "Cédula no registrada. Pide al personal del hospital que te registre.");
            var token = _tokenService.GeneratePacienteToken(paciente, PacienteSessionMinutes);
            return (true, new LoginResponseDTO
            {
                accessToken = token,
                RefreshToken = string.Empty,
                ExpiresAt = DateTime.Now.AddMinutes(PacienteSessionMinutes)
            }, null);
        }

        public async Task<LoginResponseDTO?> RefreshAcessTokenAsync(string refreshToken)
        {
            var user = await _userRepo.GetUsuarioByRefreshTokenAsync(refreshToken);
            if (user == null || user.refreshtokenexoirytime < DateTime.Now)
                return null;
            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            user.refreshtoken = newRefreshToken;
            user.refreshtokenexoirytime = DateTime.Now.AddDays(7);
            await _userRepo.SaveAsync(user);
            return new LoginResponseDTO
            {
                accessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.Now.AddMinutes(15),
                RefreshTokenExpiresAt = user.refreshtokenexoirytime
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> RegisterAsync(RegisterRequestDto dto)
        {
            var exist = await _userRepo.GetUsuarioByEmail(dto.email);
            if (exist != null)
                return (false, "User already exists");

            var usarioRole = await _roleRepo.GetByNameAsync("User");
            if (usarioRole == null)
                return (false, "User role not found");

            var user = _mapper.Map<Usuario>(dto);
            user.password = BCrypt.Net.BCrypt.EnhancedHashPassword(dto.password);
            user.RolId = usarioRole.Id;
            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var rawToken = Convert.ToBase64String(tokenBytes);
            var encodedToken = Uri.EscapeDataString(rawToken);
            user.emailconfirmationtoken = rawToken;
            // Si no hay SMTP configurado (desarrollo) la cuenta se confirma sola para poder iniciar sesion
            user.emailconfirmed = string.IsNullOrWhiteSpace(_smtp.user) || string.IsNullOrWhiteSpace(_smtp.password);
            await _userRepo.CreateAsync(user);
            await _userRepo.SaveAsync(user);
            string confirmationlink = $"https://localhost:7011/Auth/ConfirmEmail?email={Uri.EscapeDataString(user.email)}&token={encodedToken}";

            String htmlBody = $"""
           <!doctype html>
           <html>
           <p> hola,</p>
           <p>
           haz click en el siguiente boton para confirmar tu cuenta
           </p>
           <p> <a href="{confirmationlink}" style='background:#007BFF;color:white;text-decoration:none;padding:10px 20px;border-radius:5px;'>Confirmar cuenta</a></p>
           </html>
           """;
            _emailService.SendPasswordResetEmail(user.email, htmlBody);
            return (true, null);
        }

        public async Task<bool> ResendConfirmationEmailAsync(string email)
        {
            var user= await _userRepo.GetUsuarioByEmail(email);
            if (user == null || user.emailconfirmed)
                return false;
            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var rawToken = Convert.ToBase64String(tokenBytes);
            var encodedToken = Uri.EscapeDataString(rawToken);
            
            user.emailconfirmationtoken = rawToken;
            await _userRepo.SaveAsync(user);

            string confirmationlink = $"https://localhost:7011/Auth/ConfirmEmail?email={Uri.EscapeDataString(user.email)}&token={encodedToken}";

            String htmlBody = $"""
           <!doctype html>
           <html>
           <p> hola,</p>
           <p>
           haz click en el siguiente boton para confirmar tu cuenta
           </p>
           <p> <a href="{confirmationlink}" style='background:#007BFF;color:white;text-decoration:none;padding:10px 20px;border-radius:5px;'>Confirmar cuenta</a></p>
           </html>
           """;
            _emailService.SendPasswordResetEmail(user.email, htmlBody);
            return true;




        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordRequestDTO dTO)
        {
            var user = await _userRepo.GetUsuarioByEmail(dTO.email);
            if (user == null)
                return false;
            var providedToken = Uri.UnescapeDataString(dTO.token ?? string.Empty);
            var storedToken = user.resetpasswordtoken ?? string.Empty;
            // compara los tokens tnato el que esta guaradado en la bd como el que es provedido por la app
            if (!storedToken.Equals(providedToken, StringComparison.Ordinal)) return false;

            if (!user.PasswordResetTokenexpirytime.HasValue || user.PasswordResetTokenexpirytime.Value < DateTime.Now)
                return false;

            user.password = BCrypt.Net.BCrypt.EnhancedHashPassword(dTO.newPassword);
            user.resetpasswordtoken = null;
            user.PasswordResetTokenexpirytime = null;
            await _userRepo.SaveAsync(user);
            return true;
        }

        public async Task<bool> SendResetPassswordLinkAsync(string email)
        {
            var user = await _userRepo.GetUsuarioByEmail(email);
            if (user == null)
                return false;

            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var rawToken = Convert.ToBase64String(tokenBytes);
            var encodedToken = Uri.EscapeDataString(rawToken);

            user.resetpasswordtoken = rawToken;
            user.PasswordResetTokenexpirytime = DateTime.Now.AddHours(1); // Token expira en 1 hora
            await _userRepo.SaveAsync(user);

            string resetLink = $"https://localhost:7011/Auth/ResetPassword?email={Uri.EscapeDataString(user.email)}&token={encodedToken}";

            String htmlBody = $"""
           <!doctype html>
           <html>
           <p> hola,</p>
           <p>
           haz click en el siguiente boton para restablecer tu contraseña
           </p>
           <p> <a href="{resetLink}" style='background:#007BFF;color:white;text-decoration:none;padding:10px 20px;border-radius:5px;'>Restablecer contraseña</a></p>
           </html>
           """;
            _emailService.SendPasswordResetEmail(user.email, htmlBody);
            return true;
        }
    }
}
