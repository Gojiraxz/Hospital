using Hospital.DTOS;

namespace Hospital.Service
{
    // Contrato de la logica de negocio: Especialidad
    public interface IEspecialidadService
    {
        Task<datatableResponseDTO<EspecialidadTableDTO>> GetPagedAsync(DataTableRequestDTO request);
        Task<IReadOnlyList<EspecialidadTableDTO>> GetAllAsync();
        Task<SaveEspecialidadDTO?> GetByidAsync(int id);
        Task<(bool Success, String? ErrorMessage)> CreateAsync(SaveEspecialidadDTO dTO);
        Task<(bool Success, String? ErrorMessage)> UpDateAsync(SaveEspecialidadDTO dTO);
        Task<(bool Success, String? ErrorMessage)> DeleteAsync(int id);
    }
}
