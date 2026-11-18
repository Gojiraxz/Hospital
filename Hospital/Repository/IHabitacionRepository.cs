using Hospital.DTOS;
using Hospital.Models;

namespace Hospital.Repository
{
    // Contrato de acceso a datos: Habitación
    public interface IHabitacionRepository
    {
        Task<datatableResponseDTO<HabitacionTableDTO>> GetPagedAsync(DataTableRequestDTO request);
        Task<IReadOnlyList<Habitacion>> GetAllAsync();
        Task<Habitacion?> GetByIdAsync(int id);
        Task<bool> PacienteEnOtraHabitacionAsync(int pacienteId, int? excludeId = null);
        Task<bool> NumeroExisteAsync(string valor, int? excludeId = null);
        Task<Habitacion> CreateAsync(Habitacion habitacion);
        Task<bool> UpdateAsync(Habitacion habitacion);
        Task<bool> DeleteAsync(int id);
        Task<bool> HasDependentsAsync(int id);
    }
}
