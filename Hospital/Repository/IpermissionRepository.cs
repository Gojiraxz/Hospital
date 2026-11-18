using Hospital.DTOS;
using Hospital.Models;

namespace Hospital.Repository
{
    public interface IpermissionRepository
    {
        Task<datatableResponseDTO<PermissionDTO>> GetPagedAsync(DataTableRequestDTO request);
        Task<IReadOnlyList<permission>> GetAllAsync();
        Task<permission?> GetByIdAsync(int id);
        Task<permission?> GetByNameAsync(string name);
        Task<permission?> CreateAsync(string name, string? description);
        Task<bool> UpdateAsync(permission permission);
        Task<bool> DeleteAsync(int id);
        Task<bool> IsAsingnedToUsuarioAsync(int permissionId);
    }
}
