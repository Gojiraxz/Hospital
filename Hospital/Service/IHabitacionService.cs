using Hospital.DTOS;

namespace Hospital.Service
{
    // Contrato de la logica de negocio: Habitación
    public interface IHabitacionService
    {
        Task<datatableResponseDTO<HabitacionTableDTO>> GetPagedAsync(DataTableRequestDTO request);
        Task<IReadOnlyList<HabitacionTableDTO>> GetAllAsync();
        Task<SaveHabitacionDTO?> GetByidAsync(int id);
        Task<(bool Success, String? ErrorMessage)> CreateAsync(SaveHabitacionDTO dTO);
        Task<(bool Success, String? ErrorMessage)> UpDateAsync(SaveHabitacionDTO dTO);
        Task<(bool Success, String? ErrorMessage)> DeleteAsync(int id);
    }
}
