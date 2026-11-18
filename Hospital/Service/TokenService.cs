using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Hospital.Extensions;
using Hospital.Helpers;
using Hospital.Models;
using Hospital.Models.settings;

namespace Hospital.Service
{
    public class TokenService(IOptions<jwtSettings> _jwtSettings) : ITokenService
    {
        private readonly jwtSettings _jwt = _jwtSettings.Value;
        public string GenerateAccessToken(Usuario usuario)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier,usuario.Id.ToString()),
                new(ClaimTypes.Name,usuario.Nombreusuario),
                new(ClaimTypes.Email,usuario.email),
                new(ClaimTypes.Role,usuario.Role?.name ?? "User")
            };
            foreach (var permission in EffectivePermission.fromUser(usuario))
            {
                claims.Add(new Claim(ClaimsPrincipalExtensions.permissionClaimType,permission));
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _jwt.issuer,
                audience: _jwt.audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwt.tokenValidateInMinutes),
                signingCredentials: creds
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
                
               
        }

        // Token del paciente: sin permisos administrativos, solo rol "Paciente"
        public string GeneratePacienteToken(Paciente paciente, int minutes)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, "paciente:" + paciente.Id),
                new(ClaimTypes.Name, paciente.Nombre + " " + paciente.Apellido),
                new(ClaimTypes.Role, "Paciente"),
                new("pacienteId", paciente.Id.ToString())
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _jwt.issuer,
                audience: _jwt.audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(minutes),
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GeneratePasswordResetToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
    }
}
