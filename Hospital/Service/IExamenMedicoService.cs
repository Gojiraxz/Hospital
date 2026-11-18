using Hospital.DTOS;

namespace Hospital.Service
{
    // Contrato de la logica de negocio: Examen médico
    public interface IExamenMedicoService
    {
        Task<datatableResponseDTO<ExamenMedicoTableDTO>> GetPagedAsync(DataTableRequestDTO request);
        Task<SaveExamenMedicoDTO?> GetByidAsync(int id);
        Task<(bool Success, String? ErrorMessage)> CreateAsync(SaveExamenMedicoDTO dTO);
        Task<(bool Success, String? ErrorMessage)> UpDateAsync(SaveExamenMedicoDTO dTO);
        Task<(bool Success, String? ErrorMessage)> DeleteAsync(int id);
    }
}
