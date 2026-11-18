using Microsoft.EntityFrameworkCore;
using Hospital.Data;
using Hospital.Models;

namespace Hospital.Repository
{
    public class RolRepository(AppDBcontext context) : IRoleRepository
    {
        public async Task<bool> assingPermissionToRoleAsync(int roleid, int permissionid)
        {
            var exists = await context.rolespermisos.AnyAsync(x => x.RoleId == roleid && x.PermissionId == permissionid);
            if (exists) return false;
            context.rolespermisos.Add(new Rolepermission { RoleId = roleid, PermissionId = permissionid });
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<Role> createAsync(string name)
        {
            var role = new Role { name = name.Trim() };
            var entry = context.roles.Add(role);
            await context.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<IReadOnlyList<Role>> GetAllWithPermissionsAsync()
        {
            return await context.roles
                .Include(x => x.Rolepermissions)
                    .ThenInclude(x => x.Permission)
                .ToListAsync();
        }

        public async Task<Role?> GetByNameAsync(string name)
        {
            return await context.roles.FirstOrDefaultAsync(x => x.name == name);
        }

        public async Task<Role?> GetByWithPermissionAsync(int id)
        {
            return await context.roles
                .Include(x => x.Rolepermissions)
                    .ThenInclude(x => x.Permission)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<bool> RemovePermissionFromRolAsync(int roleid, int permissionid)
        {
            var rolepermission = await context.rolespermisos
                .FirstOrDefaultAsync(x => x.RoleId == roleid && x.PermissionId == permissionid);
            if (rolepermission == null) return false;
            context.rolespermisos.Remove(rolepermission);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
