using Hospital.DTOS;
using Hospital.Models;

namespace Hospital.Repository
{
    // Contrato de acceso a datos: Consulta médica
    public interface IConsultaMedicaRepository
    {
        Task<datatableResponseDTO<ConsultaMedicaTableDTO>> GetPagedAsync(DataTableRequestDTO request);
        Task<IReadOnlyList<ConsultaMedica>> GetAllAsync();
        Task<ConsultaMedica?> GetByIdAsync(int id);
        Task<ConsultaMedica> CreateAsync(ConsultaMedica consultaMedica);
        Task<bool> UpdateAsync(ConsultaMedica consultaMedica);
        Task<bool> DeleteAsync(int id);
        Task<bool> HasDependentsAsync(int id);
    }
}
