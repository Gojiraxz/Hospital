using Hospital.DTOS;
using Hospital.Models;

namespace Hospital.Repository
{
    // Contrato de acceso a datos: Receta
    public interface IRecetaRepository
    {
        Task<datatableResponseDTO<RecetaTableDTO>> GetPagedAsync(DataTableRequestDTO request);
        Task<IReadOnlyList<Receta>> GetByPacienteAsync(int pacienteId);
        Task<Receta?> GetByIdAsync(int id);
        Task<Receta> CreateAsync(Receta receta);
        Task<bool> UpdateAsync(Receta receta);
    }
}
