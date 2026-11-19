using Hospital.DTOS;

namespace Hospital.Service
{
    // Contrato de la logica de negocio: Turno
    public interface ITurnoService
    {
        Task<datatableResponseDTO<TurnoTableDTO>> GetPagedAsync(DataTableRequestDTO request);
        Task<SaveTurnoDTO?> GetByidAsync(int id);
        Task<(bool Success, String? ErrorMessage)> CreateAsync(SaveTurnoDTO dTO);
        Task<(bool Success, String? ErrorMessage)> UpDateAsync(SaveTurnoDTO dTO);
        Task<(bool Success, String? ErrorMessage)> DeleteAsync(int id);
    }
}
