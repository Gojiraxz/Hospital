using Hospital.DTOS;
using Hospital.Models;

namespace Hospital.Repository
{
    // Contrato de acceso a datos: Cita
    public interface ICitaRepository
    {
        Task<datatableResponseDTO<CitaTableDTO>> GetPagedAsync(DataTableRequestDTO request);
        Task<IReadOnlyList<Cita>> GetAllAsync();
        Task<Cita?> GetByIdAsync(int id);
        Task<IReadOnlyList<Cita>> GetByPacienteAsync(int pacienteId);
        Task<Cita> CreateAsync(Cita cita);
        Task<bool> UpdateAsync(Cita cita);
        Task<bool> DeleteAsync(int id);
        Task<bool> HasDependentsAsync(int id);
        Task<bool> DoctorOcupadoAsync(int doctorId, DateTime fecha, int? excludeId = null);
    }
}
