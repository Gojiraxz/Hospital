using Hospital.Models;

namespace Hospital.Repository
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> GetUsuarioByName(string userName);
        Task<Usuario?> GetUsuarioByEmail(string email);
        Task<Usuario?> GetUsuarioByIdwithPermissionAsync(int id);
        Task<Usuario> CreateAsync(Usuario usuario);
        bool ValidatePassword(Usuario usuario, string password);
        Task SaveAsync(Usuario user);
        Task<Usuario?> GetUsuarioByRefreshTokenAsync(string refreshToken);
        Task<bool> AssingPermissionUsuarioAsync(int usuarioId, int permissionId, bool isGranted);
        Task<bool> RemovePermissionFromUsuarioAsync(int usuarioId, int permissionId);
        Task<bool> UpdateUsuarioRolAsync(int usuarioId, int roleId);
        Task<IReadOnlyList<Usuario>> GetAllUsuariosAsync();
        Task<bool> AnyAdminAsync();
    }
}
