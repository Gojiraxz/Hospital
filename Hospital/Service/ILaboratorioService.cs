using Hospital.DTOS;

namespace Hospital.Service
{
    // Contrato de la logica de negocio: Laboratorio
    public interface ILaboratorioService
    {
        Task<datatableResponseDTO<LaboratorioTableDTO>> GetPagedAsync(DataTableRequestDTO request);
        Task<IReadOnlyList<LaboratorioTableDTO>> GetAllAsync();
        Task<SaveLaboratorioDTO?> GetByidAsync(int id);
        Task<(bool Success, String? ErrorMessage)> CreateAsync(SaveLaboratorioDTO dTO);
        Task<(bool Success, String? ErrorMessage)> UpDateAsync(SaveLaboratorioDTO dTO);
        Task<(bool Success, String? ErrorMessage)> DeleteAsync(int id);
    }
}
