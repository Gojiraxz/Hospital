using Hospital.DTOS;
using Hospital.Models;

namespace Hospital.Service
{
    public interface IRolePermissionService
    {
        
    Task<IReadOnlyList<PermissionDTO>> GetAllPermissionsAsync();
        Task<IReadOnlyList<Role>> GetAllRolesAsync();
        Task<RolwithPermissionDTO?> GetRoleByIdAsync(int roleId);
        Task<(bool Success, string? ErrorMessage)> AssignPermissionToRoleAsync(int roleId, int permissionId);
        Task<(bool Success, string? ErrorMessage)> RemovePermissionFromRoleAsync(int roleId, int permissionId);
        Task<UserPermissionDTO?> GetUserPermissionsAsync(int userId);
        Task<(bool Success, string? ErrorMessage)> AssignPermissionToUserAsync(int userId, AssingUserPermission dto);
        Task<(bool Success, string? ErrorMessage)> RemoveUserPermissionAsync(int userId, int permissionId);
        Task<(bool Success, string? ErrorMessage)> UpdateUserRoleAsync(int userId, int roleId);
        Task<datatableResponseDTO<PermissionDTO>> GetPermissionsPagedAsync(DataTableRequestDTO request);
        Task<PermissionDTO?> GetPermissionByIdAsync(int id);
        Task<(bool Success, PermissionDTO? Result, string? ErrorMessage)> CreatePermissionAsync(SavePermissionDTO dto);
        Task<(bool Success, string? ErrorMessage)> UpdatePermissionAsync(SavePermissionDTO dto);
        Task<(bool Success, string? ErrorMessage)> DeletePermissionAsync(int id);
        Task<(bool Success, RolwithPermissionDTO? Result, string? ErrorMessage)> CreateRoleAsync(CreateRolDTO dto);
        Task<IReadOnlyList<UserListItemDTO>> GetAllUsersAsync();
    }

}

