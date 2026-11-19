using Hospital.DTOS;
using Hospital.Models;
using Hospital.Repository;

namespace Hospital.Service
{
    // Logica de negocio y validaciones: Proveedor
    public class ProveedorService(IProveedorRepository repo) : IProveedorService
    {
        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(SaveProveedorDTO dTO)
        {
            if (await repo.CofeprisExisteAsync(dTO.cofepris.Trim()))
                return (false, "Ya existe un proveedor con ese registro sanitario");
            await repo.CreateAsync(new Proveedor
            {
                Nombre = dTO.nombre.Trim(),
                Correo = dTO.correo.Trim(),
                Cofepris = dTO.cofepris.Trim()
            });
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id)
        {
            if (await repo.HasDependentsAsync(id))
                return (false, "No se puede eliminar el proveedor porque tiene registros asociados");
            var deleted = await repo.DeleteAsync(id);
            return deleted ? (true, null) : (false, "Proveedor no encontrado");
        }

        // Lista para llenar los combos de otros formularios
        public async Task<IReadOnlyList<ProveedorTableDTO>> GetAllAsync()
        {
            var lista = await repo.GetAllAsync();
            return lista.Select(c => new ProveedorTableDTO
            {
                id = c.Id,
                nombre = c.Nombre,
                correo = c.Correo,
                cofepris = c.Cofepris,
                display = c.Nombre
            }).ToList();
        }

        public async Task<SaveProveedorDTO?> GetByidAsync(int id)
        {
            var proveedor = await repo.GetByIdAsync(id);
            if (proveedor == null)
                return null;
            return new SaveProveedorDTO
            {
                Id = proveedor.Id,
                nombre = proveedor.Nombre,
                correo = proveedor.Correo,
                cofepris = proveedor.Cofepris
            };
        }

        public Task<datatableResponseDTO<ProveedorTableDTO>> GetPagedAsync(DataTableRequestDTO request)
            => repo.GetPagedAsync(request);

        public async Task<(bool Success, string? ErrorMessage)> UpDateAsync(SaveProveedorDTO dTO)
        {
            if (!dTO.Id.HasValue)
                return (false, "id invalido");
            var proveedor = await repo.GetByIdAsync(dTO.Id.Value);
            if (proveedor == null)
                return (false, "Proveedor no encontrado");
            if (await repo.CofeprisExisteAsync(dTO.cofepris.Trim(), dTO.Id))
                return (false, "Ya existe un proveedor con ese registro sanitario");
            proveedor.Nombre = dTO.nombre.Trim();
            proveedor.Correo = dTO.correo.Trim();
            proveedor.Cofepris = dTO.cofepris.Trim();
            await repo.UpdateAsync(proveedor);
            return (true, null);
        }
    }
}
