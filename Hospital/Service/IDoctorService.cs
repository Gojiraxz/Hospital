using Hospital.DTOS;

namespace Hospital.Service
{
    // Contrato de la logica de negocio: Doctor
    public interface IDoctorService
    {
        Task<datatableResponseDTO<DoctorTableDTO>> GetPagedAsync(DataTableRequestDTO request);
        Task<IReadOnlyList<DoctorTableDTO>> GetAllAsync();
        Task<SaveDoctorDTO?> GetByidAsync(int id);
        Task<(bool Success, String? ErrorMessage)> CreateAsync(SaveDoctorDTO dTO);
        Task<(bool Success, String? ErrorMessage)> UpDateAsync(SaveDoctorDTO dTO);
        Task<(bool Success, String? ErrorMessage)> DeleteAsync(int id);
    }
}
