using Hospital.DTOS;
using Hospital.Models;

namespace Hospital.Repository
{
    // Contrato de acceso a datos: Enfermero
    public interface IEnfermeroRepository
    {
        Task<datatableResponseDTO<EnfermeroTableDTO>> GetPagedAsync(DataTableRequestDTO request);
        Task<IReadOnlyList<Enfermero>> GetAllAsync();
        Task<Enfermero?> GetByIdAsync(int id);
        Task<Enfermero> CreateAsync(Enfermero enfermero);
        Task<bool> UpdateAsync(Enfermero enfermero);
        Task<bool> DeleteAsync(int id);
        Task<bool> HasDependentsAsync(int id);
    }
}
