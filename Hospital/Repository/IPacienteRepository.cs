using Hospital.DTOS;
using Hospital.Models;

namespace Hospital.Repository
{
    // Contrato de acceso a datos: Paciente
    public interface IPacienteRepository
    {
        Task<datatableResponseDTO<PacienteTableDTO>> GetPagedAsync(DataTableRequestDTO request);
        Task<IReadOnlyList<Paciente>> GetAllAsync();
        Task<Paciente?> GetByIdAsync(int id);
        Task<Paciente?> GetByCedulaAsync(string cedula);
        Task<bool> CedulaExisteAsync(string valor, int? excludeId = null);
        Task<Paciente> CreateAsync(Paciente paciente);
        Task<bool> UpdateAsync(Paciente paciente);
        Task<bool> DeleteAsync(int id);
        Task<bool> HasDependentsAsync(int id);
    }
}
