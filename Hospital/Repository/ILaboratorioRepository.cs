using Hospital.DTOS;
using Hospital.Models;

namespace Hospital.Repository
{
    // Contrato de acceso a datos: Laboratorio
    public interface ILaboratorioRepository
    {
        Task<datatableResponseDTO<LaboratorioTableDTO>> GetPagedAsync(DataTableRequestDTO request);
        Task<IReadOnlyList<Laboratorio>> GetAllAsync();
        Task<Laboratorio?> GetByIdAsync(int id);
        Task<bool> NombreExisteAsync(string valor, int? excludeId = null);
        Task<Laboratorio> CreateAsync(Laboratorio laboratorio);
        Task<bool> UpdateAsync(Laboratorio laboratorio);
        Task<bool> DeleteAsync(int id);
        Task<bool> HasDependentsAsync(int id);
    }
}
