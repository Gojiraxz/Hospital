using Hospital.DTOS;
using Hospital.Models;
using Hospital.Repository;

namespace Hospital.Service
{
    // Logica de negocio y validaciones: Laboratorio
    public class LaboratorioService(ILaboratorioRepository repo) : ILaboratorioService
    {
        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(SaveLaboratorioDTO dTO)
        {
            if (await repo.NombreExisteAsync(dTO.nombre.Trim()))
                return (false, "Ya existe un laboratorio con ese nombre");
            await repo.CreateAsync(new Laboratorio
            {
                Nombre = dTO.nombre.Trim(),
                Ubicacion = dTO.ubicacion.Trim(),
                Telefono = dTO.telefono.Trim()
            });
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id)
        {
            if (await repo.HasDependentsAsync(id))
                return (false, "No se puede eliminar el laboratorio porque tiene registros asociados");
            var deleted = await repo.DeleteAsync(id);
            return deleted ? (true, null) : (false, "Laboratorio no encontrado");
        }

        // Lista para llenar los combos de otros formularios
        public async Task<IReadOnlyList<LaboratorioTableDTO>> GetAllAsync()
        {
            var lista = await repo.GetAllAsync();
            return lista.Select(c => new LaboratorioTableDTO
            {
                id = c.Id,
                nombre = c.Nombre,
                ubicacion = c.Ubicacion,
                telefono = c.Telefono,
                display = c.Nombre
            }).ToList();
        }

        public async Task<SaveLaboratorioDTO?> GetByidAsync(int id)
        {
            var laboratorio = await repo.GetByIdAsync(id);
            if (laboratorio == null)
                return null;
            return new SaveLaboratorioDTO
            {
                Id = laboratorio.Id,
                nombre = laboratorio.Nombre,
                ubicacion = laboratorio.Ubicacion,
                telefono = laboratorio.Telefono
            };
        }

        public Task<datatableResponseDTO<LaboratorioTableDTO>> GetPagedAsync(DataTableRequestDTO request)
            => repo.GetPagedAsync(request);

        public async Task<(bool Success, string? ErrorMessage)> UpDateAsync(SaveLaboratorioDTO dTO)
        {
            if (!dTO.Id.HasValue)
                return (false, "id invalido");
            var laboratorio = await repo.GetByIdAsync(dTO.Id.Value);
            if (laboratorio == null)
                return (false, "Laboratorio no encontrado");
            if (await repo.NombreExisteAsync(dTO.nombre.Trim(), dTO.Id))
                return (false, "Ya existe un laboratorio con ese nombre");
            laboratorio.Nombre = dTO.nombre.Trim();
            laboratorio.Ubicacion = dTO.ubicacion.Trim();
            laboratorio.Telefono = dTO.telefono.Trim();
            await repo.UpdateAsync(laboratorio);
            return (true, null);
        }
    }
}
