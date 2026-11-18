using Hospital.DTOS;

namespace Hospital.Service
{
    // Contrato de la logica de negocio: Factura
    public interface IFacturaService
    {
        Task<datatableResponseDTO<FacturaTableDTO>> GetPagedAsync(DataTableRequestDTO request);
        Task<IReadOnlyList<FacturaTableDTO>> GetAllAsync();
        Task<SaveFacturaDTO?> GetByidAsync(int id);
        Task<(bool Success, String? ErrorMessage)> CreateAsync(SaveFacturaDTO dTO);
        Task<(bool Success, String? ErrorMessage)> UpDateAsync(SaveFacturaDTO dTO);
        Task<(bool Success, String? ErrorMessage)> DeleteAsync(int id);
    }
}
