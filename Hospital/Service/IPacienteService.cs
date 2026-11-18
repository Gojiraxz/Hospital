using Hospital.DTOS;

namespace Hospital.Service
{
    // Contrato de la logica de negocio: Paciente
    public interface IPacienteService
    {
        Task<datatableResponseDTO<PacienteTableDTO>> GetPagedAsync(DataTableRequestDTO request);
        Task<IReadOnlyList<PacienteTableDTO>> GetAllAsync();
        Task<SavePacienteDTO?> GetByidAsync(int id);
        Task<(bool Success, String? ErrorMessage)> CreateAsync(SavePacienteDTO dTO);
        Task<(bool Success, String? ErrorMessage)> UpDateAsync(SavePacienteDTO dTO);
        Task<(bool Success, String? ErrorMessage)> DeleteAsync(int id);
    }
}
