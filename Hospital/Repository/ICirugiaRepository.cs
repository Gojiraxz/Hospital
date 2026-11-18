using Hospital.DTOS;
using Hospital.Models;

namespace Hospital.Repository
{
    // Contrato de acceso a datos: Cirugía
    public interface ICirugiaRepository
    {
        Task<datatableResponseDTO<CirugiaTableDTO>> GetPagedAsync(DataTableRequestDTO request);
        Task<Cirugia?> GetByIdAsync(int id);
        Task<Cirugia> CreateAsync(Cirugia cirugia);
        Task<bool> UpdateAsync(Cirugia cirugia);
        Task<bool> DeleteAsync(int id);
        Task<bool> HabitacionOcupadaAsync(int habitacionId, DateTime fecha, int? excludeId = null);
    }
}
