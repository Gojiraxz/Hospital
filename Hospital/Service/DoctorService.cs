using Hospital.DTOS;
using Hospital.Models;
using Hospital.Repository;

namespace Hospital.Service
{
    // Logica de negocio y validaciones: Doctor
    public class DoctorService(IDoctorRepository repo, IEspecialidadRepository repoEspecialidad) : IDoctorService
    {
        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(SaveDoctorDTO dTO)
        {
            if (await repoEspecialidad.GetByIdAsync(dTO.especialidadId) == null)
                return (false, "La especialidad seleccionada no existe");
            await repo.CreateAsync(new Doctor
            {
                Nombre = dTO.nombre.Trim(),
                Apellido = dTO.apellido.Trim(),
                EspecialidadId = dTO.especialidadId
            });
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id)
        {
            if (await repo.HasDependentsAsync(id))
                return (false, "No se puede eliminar el doctor porque tiene registros asociados");
            var deleted = await repo.DeleteAsync(id);
            return deleted ? (true, null) : (false, "Doctor no encontrado");
        }

        // Lista para llenar los combos de otros formularios
        public async Task<IReadOnlyList<DoctorTableDTO>> GetAllAsync()
        {
            var lista = await repo.GetAllAsync();
            return lista.Select(c => new DoctorTableDTO
            {
                id = c.Id,
                nombre = c.Nombre,
                apellido = c.Apellido,
                especialidadId = c.EspecialidadId,
                especialidadName = c.Especialidad.Nombre,
                display = "Dr. " + c.Nombre + " " + c.Apellido
            }).ToList();
        }

        public async Task<SaveDoctorDTO?> GetByidAsync(int id)
        {
            var doctor = await repo.GetByIdAsync(id);
            if (doctor == null)
                return null;
            return new SaveDoctorDTO
            {
                Id = doctor.Id,
                nombre = doctor.Nombre,
                apellido = doctor.Apellido,
                especialidadId = doctor.EspecialidadId
            };
        }

        public Task<datatableResponseDTO<DoctorTableDTO>> GetPagedAsync(DataTableRequestDTO request)
            => repo.GetPagedAsync(request);

        public async Task<(bool Success, string? ErrorMessage)> UpDateAsync(SaveDoctorDTO dTO)
        {
            if (!dTO.Id.HasValue)
                return (false, "id invalido");
            var doctor = await repo.GetByIdAsync(dTO.Id.Value);
            if (doctor == null)
                return (false, "Doctor no encontrado");
            if (await repoEspecialidad.GetByIdAsync(dTO.especialidadId) == null)
                return (false, "La especialidad seleccionada no existe");
            doctor.Nombre = dTO.nombre.Trim();
            doctor.Apellido = dTO.apellido.Trim();
            doctor.EspecialidadId = dTO.especialidadId;
            await repo.UpdateAsync(doctor);
            return (true, null);
        }
    }
}
