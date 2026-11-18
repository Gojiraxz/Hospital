using Hospital.DTOS;
using Hospital.Models;
using Hospital.Repository;

namespace Hospital.Service
{
    // Logica de negocio y validaciones: Enfermero
    public class EnfermeroService(IEnfermeroRepository repo, IEspecialidadRepository repoEspecialidad) : IEnfermeroService
    {
        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(SaveEnfermeroDTO dTO)
        {
            if (await repoEspecialidad.GetByIdAsync(dTO.especialidadId) == null)
                return (false, "La especialidad seleccionada no existe");
            await repo.CreateAsync(new Enfermero
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
                return (false, "No se puede eliminar el enfermero porque tiene registros asociados");
            var deleted = await repo.DeleteAsync(id);
            return deleted ? (true, null) : (false, "Enfermero no encontrado");
        }

        // Lista para llenar los combos de otros formularios
        public async Task<IReadOnlyList<EnfermeroTableDTO>> GetAllAsync()
        {
            var lista = await repo.GetAllAsync();
            return lista.Select(c => new EnfermeroTableDTO
            {
                id = c.Id,
                nombre = c.Nombre,
                apellido = c.Apellido,
                especialidadId = c.EspecialidadId,
                especialidadName = c.Especialidad.Nombre,
                display = c.Nombre + " " + c.Apellido
            }).ToList();
        }

        public async Task<SaveEnfermeroDTO?> GetByidAsync(int id)
        {
            var enfermero = await repo.GetByIdAsync(id);
            if (enfermero == null)
                return null;
            return new SaveEnfermeroDTO
            {
                Id = enfermero.Id,
                nombre = enfermero.Nombre,
                apellido = enfermero.Apellido,
                especialidadId = enfermero.EspecialidadId
            };
        }

        public Task<datatableResponseDTO<EnfermeroTableDTO>> GetPagedAsync(DataTableRequestDTO request)
            => repo.GetPagedAsync(request);

        public async Task<(bool Success, string? ErrorMessage)> UpDateAsync(SaveEnfermeroDTO dTO)
        {
            if (!dTO.Id.HasValue)
                return (false, "id invalido");
            var enfermero = await repo.GetByIdAsync(dTO.Id.Value);
            if (enfermero == null)
                return (false, "Enfermero no encontrado");
            if (await repoEspecialidad.GetByIdAsync(dTO.especialidadId) == null)
                return (false, "La especialidad seleccionada no existe");
            enfermero.Nombre = dTO.nombre.Trim();
            enfermero.Apellido = dTO.apellido.Trim();
            enfermero.EspecialidadId = dTO.especialidadId;
            await repo.UpdateAsync(enfermero);
            return (true, null);
        }
    }
}
