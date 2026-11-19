using Hospital.DTOS;

namespace Hospital.Service
{
    // Contrato de la logica de negocio: Detalle de factura
    public interface IDetalleFacturaService
    {
        Task<datatableResponseDTO<DetalleFacturaTableDTO>> GetPagedAsync(DataTableRequestDTO request);
        Task<SaveDetalleFacturaDTO?> GetByidAsync(int id);
        Task<(bool Success, String? ErrorMessage)> CreateAsync(SaveDetalleFacturaDTO dTO);
        Task<(bool Success, String? ErrorMessage)> UpDateAsync(SaveDetalleFacturaDTO dTO);
        Task<(bool Success, String? ErrorMessage)> DeleteAsync(int id);
    }
}
