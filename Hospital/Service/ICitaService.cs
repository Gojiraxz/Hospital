using Hospital.DTOS;

namespace Hospital.Service
{
    // Contrato de la logica de negocio: Cita
    public interface ICitaService
    {
        Task<datatableResponseDTO<CitaTableDTO>> GetPagedAsync(DataTableRequestDTO request);
        Task<IReadOnlyList<CitaTableDTO>> GetAllAsync();
        Task<SaveCitaDTO?> GetByidAsync(int id);
        Task<IReadOnlyList<CitaTableDTO>> GetByPacienteAsync(int pacienteId);
        Task<(bool Success, String? ErrorMessage)> CreateAsync(SaveCitaDTO dTO);
        Task<(bool Success, String? ErrorMessage)> UpDateAsync(SaveCitaDTO dTO);
        Task<(bool Success, String? ErrorMessage)> DeleteAsync(int id);
    }
}
