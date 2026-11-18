using Hospital.DTOS;
using Hospital.Models;

namespace Hospital.Repository
{
    // Contrato de acceso a datos: Medicamento
    public interface IMedicamentoRepository
    {
        Task<datatableResponseDTO<MedicamentoTableDTO>> GetPagedAsync(DataTableRequestDTO request);
        Task<IReadOnlyList<Medicamento>> GetAllAsync();
        Task<Medicamento?> GetByIdAsync(int id);
        Task<bool> NombreExisteAsync(string valor, int? excludeId = null);
        Task<Medicamento> CreateAsync(Medicamento medicamento);
        Task<bool> UpdateAsync(Medicamento medicamento);
        Task<bool> DeleteAsync(int id);
        Task<bool> HasDependentsAsync(int id);
    }
}
