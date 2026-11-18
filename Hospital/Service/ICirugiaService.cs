using Hospital.DTOS;

namespace Hospital.Service
{
    // Contrato de la logica de negocio: Cirugía
    public interface ICirugiaService
    {
        Task<datatableResponseDTO<CirugiaTableDTO>> GetPagedAsync(DataTableRequestDTO request);
        Task<SaveCirugiaDTO?> GetByidAsync(int id);
        Task<(bool Success, String? ErrorMessage)> CreateAsync(SaveCirugiaDTO dTO);
        Task<(bool Success, String? ErrorMessage)> UpDateAsync(SaveCirugiaDTO dTO);
        Task<(bool Success, String? ErrorMessage)> DeleteAsync(int id);
    }
}
