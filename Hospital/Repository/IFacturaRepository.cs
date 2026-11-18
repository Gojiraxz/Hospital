using Hospital.DTOS;
using Hospital.Models;

namespace Hospital.Repository
{
    // Contrato de acceso a datos: Factura
    public interface IFacturaRepository
    {
        Task<datatableResponseDTO<FacturaTableDTO>> GetPagedAsync(DataTableRequestDTO request);
        Task<IReadOnlyList<Factura>> GetAllAsync();
        Task<Factura?> GetByIdAsync(int id);
        Task<Factura> CreateAsync(Factura factura);
        Task<bool> UpdateAsync(Factura factura);
        Task<bool> DeleteAsync(int id);
        Task<bool> HasDependentsAsync(int id);
        Task RecalcularTotalAsync(int facturaId);
    }
}
