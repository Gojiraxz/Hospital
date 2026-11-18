using Hospital.DTOS;
using Hospital.Models;
using Hospital.Repository;

namespace Hospital.Service
{
    // Logica de negocio y validaciones: Especialidad
    public class EspecialidadService(IEspecialidadRepository repo) : IEspecialidadService
    {
        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(SaveEspecialidadDTO dTO)
        {
            if (await repo.NombreExisteAsync(dTO.nombre.Trim()))
                return (false, "Ya existe una especialidad con ese nombre");
            await repo.CreateAsync(new Especialidad
            {
                Nombre = dTO.nombre.Trim()
            });
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id)
        {
            if (await repo.HasDependentsAsync(id))
                return (false, "No se puede eliminar la especialidad porque tiene registros asociados");
            var deleted = await repo.DeleteAsync(id);
            return deleted ? (true, null) : (false, "Especialidad no encontrada");
        }

        // Lista para llenar los combos de otros formularios
        public async Task<IReadOnlyList<EspecialidadTableDTO>> GetAllAsync()
        {
            var lista = await repo.GetAllAsync();
            return lista.Select(c => new EspecialidadTableDTO
            {
                id = c.Id,
                nombre = c.Nombre,
                display = c.Nombre
            }).ToList();
        }

        public async Task<SaveEspecialidadDTO?> GetByidAsync(int id)
        {
            var especialidad = await repo.GetByIdAsync(id);
            if (especialidad == null)
                return null;
            return new SaveEspecialidadDTO
            {
                Id = especialidad.Id,
                nombre = especialidad.Nombre
            };
        }

        public Task<datatableResponseDTO<EspecialidadTableDTO>> GetPagedAsync(DataTableRequestDTO request)
            => repo.GetPagedAsync(request);

        public async Task<(bool Success, string? ErrorMessage)> UpDateAsync(SaveEspecialidadDTO dTO)
        {
            if (!dTO.Id.HasValue)
                return (false, "id invalido");
            var especialidad = await repo.GetByIdAsync(dTO.Id.Value);
            if (especialidad == null)
                return (false, "Especialidad no encontrada");
            if (await repo.NombreExisteAsync(dTO.nombre.Trim(), dTO.Id))
                return (false, "Ya existe una especialidad con ese nombre");
            especialidad.Nombre = dTO.nombre.Trim();
            await repo.UpdateAsync(especialidad);
            return (true, null);
        }
    }
}
