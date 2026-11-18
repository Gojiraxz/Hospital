using Hospital.DTOS;
using Hospital.Models;

namespace Hospital.Repository
{
    // Contrato de acceso a datos: Examen médico
    public interface IExamenMedicoRepository
    {
        Task<datatableResponseDTO<ExamenMedicoTableDTO>> GetPagedAsync(DataTableRequestDTO request);
        Task<ExamenMedico?> GetByIdAsync(int id);
        Task<ExamenMedico> CreateAsync(ExamenMedico examenMedico);
        Task<bool> UpdateAsync(ExamenMedico examenMedico);
        Task<bool> DeleteAsync(int id);
    }
}
