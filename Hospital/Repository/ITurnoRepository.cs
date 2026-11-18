using Hospital.DTOS;
using Hospital.Models;

namespace Hospital.Repository
{
    // Contrato de acceso a datos: Turno
    public interface ITurnoRepository
    {
        Task<datatableResponseDTO<TurnoTableDTO>> GetPagedAsync(DataTableRequestDTO request);
        Task<Turno?> GetByIdAsync(int id);
        Task<Turno> CreateAsync(Turno turno);
        Task<bool> UpdateAsync(Turno turno);
        Task<bool> DeleteAsync(int id);
    }
}
