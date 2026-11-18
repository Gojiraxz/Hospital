using Hospital.DTOS;
using Hospital.Models;

namespace Hospital.Repository
{
    // Contrato de acceso a datos: Doctor
    public interface IDoctorRepository
    {
        Task<datatableResponseDTO<DoctorTableDTO>> GetPagedAsync(DataTableRequestDTO request);
        Task<IReadOnlyList<Doctor>> GetAllAsync();
        Task<Doctor?> GetByIdAsync(int id);
        Task<Doctor> CreateAsync(Doctor doctor);
        Task<bool> UpdateAsync(Doctor doctor);
        Task<bool> DeleteAsync(int id);
        Task<bool> HasDependentsAsync(int id);
    }
}
