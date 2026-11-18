using Hospital.DTOS;
using Hospital.Models;

namespace Hospital.Repository
{
    // Contrato de acceso a datos: Detalle de factura
    public interface IDetalleFacturaRepository
    {
        Task<datatableResponseDTO<DetalleFacturaTableDTO>> GetPagedAsync(DataTableRequestDTO request);
        Task<DetalleFactura?> GetByIdAsync(int id);
        Task<DetalleFactura> CreateAsync(DetalleFactura detalleFactura);
        Task<bool> UpdateAsync(DetalleFactura detalleFactura);
        Task<bool> DeleteAsync(int id);
    }
}
