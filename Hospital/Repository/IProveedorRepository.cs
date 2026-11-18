using Hospital.DTOS;
using Hospital.Models;

namespace Hospital.Repository
{
    // Contrato de acceso a datos: Proveedor
    public interface IProveedorRepository
    {
        Task<datatableResponseDTO<ProveedorTableDTO>> GetPagedAsync(DataTableRequestDTO request);
        Task<IReadOnlyList<Proveedor>> GetAllAsync();
        Task<Proveedor?> GetByIdAsync(int id);
        Task<bool> CofeprisExisteAsync(string valor, int? excludeId = null);
        Task<Proveedor> CreateAsync(Proveedor proveedor);
        Task<bool> UpdateAsync(Proveedor proveedor);
        Task<bool> DeleteAsync(int id);
        Task<bool> HasDependentsAsync(int id);
    }
}
