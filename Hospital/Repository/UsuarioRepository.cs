using Microsoft.EntityFrameworkCore;
using Hospital.Data;
using Hospital.Models;

namespace Hospital.Repository
{
    public class UsuarioRepository(AppDBcontext context) : IUsuarioRepository
    {
        // Consulta base: usuario + rol + permisos del rol + permisos directos del usuario
        private IQueryable<Usuario> UserWithPermission => context.usuarios
            .Include(u => u.Role)
                .ThenInclude(r => r.Rolepermissions)
                    .ThenInclude(rp => rp.Permission)
            .Include(u => u.UsuarioPermisos)
                .ThenInclude(up => up.Permission);

        public async Task<bool> AssingPermissionUsuarioAsync(int usuarioId, int permissionId, bool isGranted)
        {
            var userPermission = await context.usuariospermisos
                .FirstOrDefaultAsync(up => up.Usuarioid == usuarioId && up.PermissionId == permissionId);

            if (userPermission != null)
            {
                userPermission.isGranted = isGranted;
            }
            else
            {
                context.usuariospermisos.Add(new UsuarioPermiso
                {
                    Usuarioid = usuarioId,
                    PermissionId = permissionId,
                    isGranted = isGranted
                });
            }

            await context.SaveChangesAsync();
            return true;
        }

        // Indica si ya existe al menos una cuenta con rol Admin
        public async Task<bool> AnyAdminAsync()
        {
            return await context.usuarios.AnyAsync(u => u.Role.name == "Admin");
        }

        public async Task<Usuario> CreateAsync(Usuario usuario)
        {
            var entry = await context.usuarios.AddAsync(usuario);
            await context.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<IReadOnlyList<Usuario>> GetAllUsuariosAsync()
        {
            return await context.usuarios
                .Include(u => u.Role)
                .OrderBy(u => u.Nombreusuario)
                .ToListAsync();
        }

        public async Task<Usuario?> GetUsuarioByEmail(string email)
        {
            return await UserWithPermission.FirstOrDefaultAsync(u => u.email == email);
        }

        public async Task<Usuario?> GetUsuarioByIdwithPermissionAsync(int id)
        {
            return await UserWithPermission.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<Usuario?> GetUsuarioByRefreshTokenAsync(string refreshToken)
        {
            return await UserWithPermission.FirstOrDefaultAsync(u => u.refreshtoken == refreshToken);
        }

        public async Task<Usuario?> GetUsuarioByName(string userName)
        {
            return await UserWithPermission.FirstOrDefaultAsync(u => u.Nombreusuario == userName);
        }

        public async Task<bool> RemovePermissionFromUsuarioAsync(int usuarioId, int permissionId)
        {
            var userPermission = await context.usuariospermisos
                .FirstOrDefaultAsync(up => up.Usuarioid == usuarioId && up.PermissionId == permissionId);
            if (userPermission == null) return false;

            context.usuariospermisos.Remove(userPermission);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }

        public async Task<bool> UpdateUsuarioRolAsync(int usuarioId, int roleId)
        {
            var user = await context.usuarios.FindAsync(usuarioId);
            if (user == null) return false;

            var roleExists = await context.roles.AnyAsync(r => r.Id == roleId);
            if (!roleExists) return false;

            user.RolId = roleId;
            await context.SaveChangesAsync();
            return true;
        }

        public bool ValidatePassword(Usuario usuario, string password)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(password, usuario.password);
        }

        public async Task SaveAsync(Usuario user)
        {
            await context.SaveChangesAsync();
        }
    }
}
