using Hospital.DTOS;
using Hospital.Models;

namespace Hospital.Repository
{
    // Contrato de acceso a datos: Especialidad
    public interface IEspecialidadRepository
    {
        Task<datatableResponseDTO<EspecialidadTableDTO>> GetPagedAsync(DataTableRequestDTO request);
        Task<IReadOnlyList<Especialidad>> GetAllAsync();
        Task<Especialidad?> GetByIdAsync(int id);
        Task<bool> NombreExisteAsync(string valor, int? excludeId = null);
        Task<Especialidad> CreateAsync(Especialidad especialidad);
        Task<bool> UpdateAsync(Especialidad especialidad);
        Task<bool> DeleteAsync(int id);
        Task<bool> HasDependentsAsync(int id);
    }
}
