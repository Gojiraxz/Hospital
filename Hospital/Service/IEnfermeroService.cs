using Hospital.DTOS;

namespace Hospital.Service
{
    // Contrato de la logica de negocio: Enfermero
    public interface IEnfermeroService
    {
        Task<datatableResponseDTO<EnfermeroTableDTO>> GetPagedAsync(DataTableRequestDTO request);
        Task<IReadOnlyList<EnfermeroTableDTO>> GetAllAsync();
        Task<SaveEnfermeroDTO?> GetByidAsync(int id);
        Task<(bool Success, String? ErrorMessage)> CreateAsync(SaveEnfermeroDTO dTO);
        Task<(bool Success, String? ErrorMessage)> UpDateAsync(SaveEnfermeroDTO dTO);
        Task<(bool Success, String? ErrorMessage)> DeleteAsync(int id);
    }
}
