using Hospital.DTOS;

namespace Hospital.Service
{
    // Contrato de la logica de negocio: Proveedor
    public interface IProveedorService
    {
        Task<datatableResponseDTO<ProveedorTableDTO>> GetPagedAsync(DataTableRequestDTO request);
        Task<IReadOnlyList<ProveedorTableDTO>> GetAllAsync();
        Task<SaveProveedorDTO?> GetByidAsync(int id);
        Task<(bool Success, String? ErrorMessage)> CreateAsync(SaveProveedorDTO dTO);
        Task<(bool Success, String? ErrorMessage)> UpDateAsync(SaveProveedorDTO dTO);
        Task<(bool Success, String? ErrorMessage)> DeleteAsync(int id);
    }
}
