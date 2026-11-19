using Hospital.DTOS;

namespace Hospital.Service
{
    // Contrato de la logica de negocio: Consulta médica
    public interface IConsultaMedicaService
    {
        Task<datatableResponseDTO<ConsultaMedicaTableDTO>> GetPagedAsync(DataTableRequestDTO request);
        Task<IReadOnlyList<ConsultaMedicaTableDTO>> GetAllAsync();
        Task<SaveConsultaMedicaDTO?> GetByidAsync(int id);
        Task<(bool Success, String? ErrorMessage)> CreateAsync(SaveConsultaMedicaDTO dTO);
        Task<(bool Success, String? ErrorMessage)> UpDateAsync(SaveConsultaMedicaDTO dTO);
        Task<(bool Success, String? ErrorMessage)> DeleteAsync(int id);
    }
}
