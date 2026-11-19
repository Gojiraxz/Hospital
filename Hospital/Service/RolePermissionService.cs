using Hospital.DTOS;
using Hospital.Helpers;
using Hospital.Models;
using Hospital.Repository;

namespace Hospital.Service
{
    public class RolePermissionService(IRoleRepository roleRepo, IpermissionRepository permissionRepo, IUsuarioRepository userRepo) : IRolePermissionService
    {
        public async Task<(bool Success, string? ErrorMessage)> AssignPermissionToRoleAsync(int roleId, int permissionId)
        {
            var role = await roleRepo.GetByWithPermissionAsync(roleId);
            if (role == null)
                return (false, "Rol no encontrado");
            var permission = await permissionRepo.GetByIdAsync(permissionId);
            if (permission == null)
                return (false, "Permiso no encontrado");
            var assigned = await roleRepo.assingPermissionToRoleAsync(roleId, permissionId);
            if (!assigned)
                return (false, "EL rol ya tiene asignado ese permiso");
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> AssignPermissionToUserAsync(int userId, AssingUserPermission dto)
        {
            var user = await userRepo.GetUsuarioByIdwithPermissionAsync(userId);
            if (user == null)
                return (false, "Usuario no encontrado");

            var permission = await permissionRepo.GetByIdAsync(dto.permissionId);
            if (permission == null)
                return (false, "Permiso no encontrado");
            await userRepo.AssingPermissionUsuarioAsync(userId, dto.permissionId, dto.isgranted);
            return (true, null);
        }

        public async Task<(bool Success, PermissionDTO? Result, string? ErrorMessage)> CreatePermissionAsync(SavePermissionDTO dto)
        {
            var name = dto.name.Trim().ToLowerInvariant();
            var existing = await permissionRepo.GetByNameAsync(name);
            if (existing != null)
                return (false, null, "Ya existe un permiso con ese nombre");
            var permission = await permissionRepo.CreateAsync(name, dto.Descripcion);
            if (permission == null)
                return (false, null, "No se pudo crear el permiso");
            return (true, new PermissionDTO
            {
                Id = permission.Id,
                Name = permission.name,
                Description = permission.Description,
            }, null);
        }

        public async Task<(bool Success, RolwithPermissionDTO? Result, string? ErrorMessage)> CreateRoleAsync(CreateRolDTO dto)
        {
            var existing = await roleRepo.GetByNameAsync(dto.name.Trim());
            if (existing != null)
                return (false, null, "Ya existe un rol con ese nombre");

            var role = await roleRepo.createAsync(dto.name);
            return (true, new RolwithPermissionDTO
            {
                id = role.Id,
                name = role.name,
                Permissions = []
            }, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeletePermissionAsync(int id)
        {
            var permission = await permissionRepo.GetByIdAsync(id);
            if (permission == null)
                return (false, "Permiso no encontrado");
            if (await permissionRepo.IsAsingnedToUsuarioAsync(id))
                return (false, "No se puede eliminar el permiso asignado a uno o mas usuarios");
            await permissionRepo.DeleteAsync(id);
            return (true, null);
        }

        public async Task<IReadOnlyList<PermissionDTO>> GetAllPermissionsAsync()
        {
            var permissions = await permissionRepo.GetAllAsync();
            return permissions.Select(p => new PermissionDTO
            {
                Id = p.Id,
                Name = p.name,
                Description = p.Description,
            }).ToList();
        }

        public async Task<IReadOnlyList<Role>> GetAllRolesAsync()
        {
            return await roleRepo.GetAllWithPermissionsAsync();
        }

        public async Task<IReadOnlyList<UserListItemDTO>> GetAllUsersAsync()
        {
            var users = await userRepo.GetAllUsuariosAsync();
            return users.Select(u => new UserListItemDTO
            {
                Id = u.Id,
                username = u.Nombreusuario,
                email = u.email,
                RoleName = u.Role?.name ?? string.Empty
            }).ToList();
        }

        public async Task<PermissionDTO?> GetPermissionByIdAsync(int id)
        {
            var permission = await permissionRepo.GetByIdAsync(id);
            if (permission == null)
                return null;
            return new PermissionDTO
            {
                Id = permission.Id,
                Name = permission.name,
                Description = permission.Description,
            };
        }

        public async Task<datatableResponseDTO<PermissionDTO>> GetPermissionsPagedAsync(DataTableRequestDTO request)
        {
            return await permissionRepo.GetPagedAsync(request);
        }

        public async Task<RolwithPermissionDTO?> GetRoleByIdAsync(int roleId)
        {
            var role = await roleRepo.GetByWithPermissionAsync(roleId);
            return role == null ? null : MapRole(role);
        }

        public async Task<UserPermissionDTO?> GetUserPermissionsAsync(int userId)
        {
            var user = await userRepo.GetUsuarioByIdwithPermissionAsync(userId);
            if (user == null)
                return null;
            return new UserPermissionDTO
            {
                userId = user.Id,
                UserName = user.Nombreusuario,
                Email = user.email,
                RoleId = user.RolId,
                RoleName = user.Role?.name ?? string.Empty,
                EffectivePermission = EffectivePermission.fromUser(user).ToList(),
                DirectPermission = user.UsuarioPermisos.Select(up => new UserPermissionOverride
                {
                    permissionId = up.PermissionId,
                    permissionName = up.Permission.name,
                    IsGranted = up.isGranted
                }).ToList(),
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> RemovePermissionFromRoleAsync(int roleId, int permissionId)
        {
            var removed = await roleRepo.RemovePermissionFromRolAsync(roleId, permissionId);
            if (!removed)
                return (false, "El rol no tiene asignado este permiso");
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> RemoveUserPermissionAsync(int userId, int permissionId)
        {
            var removed = await userRepo.RemovePermissionFromUsuarioAsync(userId, permissionId);
            if (!removed)
                return (false, "El usuario no tiene una asignaccion directa para este permiso");
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdatePermissionAsync(SavePermissionDTO dto)
        {
            if (!dto.id.HasValue)
                return (false, "Id de permiso invalido");
            var permission = await permissionRepo.GetByIdAsync(dto.id.Value);
            if (permission == null)
                return (false, "Permiso no encontrado");
            var name = dto.name.Trim().ToLowerInvariant();
            var duplicate = await permissionRepo.GetByNameAsync(name);
            if (duplicate != null && duplicate.Id != permission.Id)
                return (false, "Ya existe un permiso con ese nombre");
            permission.name = name;
            permission.Description = dto.Descripcion?.Trim();
            await permissionRepo.UpdateAsync(permission);
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateUserRoleAsync(int userId, int roleId)
        {
            var updated = await userRepo.UpdateUsuarioRolAsync(userId, roleId);
            if (!updated)
                return (false, "Usuario o rol no encontrado");
            return (true, null);
        }

        private static RolwithPermissionDTO MapRole(Role role) => new()
        {
            id = role.Id,
            name = role.name,
            Permissions = role.Rolepermissions.Select(rp => new PermissionDTO
            {
                Id = rp.PermissionId,
                Name = rp.Permission.name,
                Description = rp.Permission.Description,
            }).ToList()
        };
    }
}
