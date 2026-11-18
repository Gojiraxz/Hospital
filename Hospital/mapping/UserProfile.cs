using AutoMapper;
using Hospital.DTOS;
using Hospital.Models;

namespace Hospital.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<Usuario, LoginResponseDTO>()
                .ForMember(d => d.accessToken, opt => opt.Ignore())
                .ForMember(d => d.RefreshToken, opt => opt.Ignore())
                .ForMember(d => d.ExpiresAt, opt => opt.Ignore())
                .ForMember(d => d.RefreshTokenExpiresAt, opt => opt.MapFrom(src => src.refreshtokenexoirytime));

            CreateMap<RegisterRequestDto, Usuario>()
                .ForMember(d => d.Nombreusuario, opt => opt.MapFrom(src => src.username))
                .ForMember(d => d.RolId, opt => opt.MapFrom(src => src.roleId))
                .ForMember(d => d.password, opt => opt.Ignore())
                .ForMember(d => d.emailconfirmed, opt => opt.MapFrom(src => false))
                .ForMember(d => d.emailconfirmationtoken, opt => opt.Ignore())
                .ForMember(d => d.refreshtoken, opt => opt.Ignore())
                .ForMember(d => d.refreshtokenexoirytime, opt => opt.Ignore())
                .ForMember(d => d.resetpasswordtoken, opt => opt.Ignore())
                .ForMember(d => d.PasswordResetTokenexpirytime, opt => opt.Ignore())
                .ForMember(d => d.Role, opt => opt.Ignore())
                .ForMember(d => d.UsuarioPermisos, opt => opt.Ignore());
        }
    }
}
