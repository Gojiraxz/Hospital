using Hospital.Models;

namespace Hospital.Repository
{
    public interface IRoleRepository
    {
        Task<IReadOnlyList<Role>> GetAllWithPermissionsAsync();
        Task<Role?> GetByWithPermissionAsync(int id);
        Task<Role?> GetByNameAsync(string name);
        Task<Role> createAsync(string name);
        Task<bool> assingPermissionToRoleAsync(int roleid, int permissionid);
        Task<bool> RemovePermissionFromRolAsync(int roleid, int permissionid);
    }
}
