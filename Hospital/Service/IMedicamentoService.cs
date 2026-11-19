using Hospital.DTOS;

namespace Hospital.Service
{
    // Contrato de la logica de negocio: Medicamento
    public interface IMedicamentoService
    {
        Task<datatableResponseDTO<MedicamentoTableDTO>> GetPagedAsync(DataTableRequestDTO request);
        Task<IReadOnlyList<MedicamentoTableDTO>> GetAllAsync();
        Task<SaveMedicamentoDTO?> GetByidAsync(int id);
        Task<(bool Success, String? ErrorMessage)> CreateAsync(SaveMedicamentoDTO dTO);
        Task<(bool Success, String? ErrorMessage)> UpDateAsync(SaveMedicamentoDTO dTO);
        Task<(bool Success, String? ErrorMessage)> DeleteAsync(int id);
    }
}
