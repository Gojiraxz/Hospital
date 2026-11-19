using Hospital.DTOS;
using Hospital.Models;
using Hospital.Repository;

namespace Hospital.Service
{
    // Logica de negocio y validaciones: Medicamento
    public class MedicamentoService(IMedicamentoRepository repo, IProveedorRepository repoProveedor) : IMedicamentoService
    {
        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(SaveMedicamentoDTO dTO)
        {
            if (await repo.NombreExisteAsync(dTO.nombre.Trim()))
                return (false, "Ya existe un medicamento con ese nombre");
            if (await repoProveedor.GetByIdAsync(dTO.proveedorId) == null)
                return (false, "El proveedor seleccionado no existe");
            await repo.CreateAsync(new Medicamento
            {
                Nombre = dTO.nombre.Trim(),
                Descripcion = dTO.descripcion.Trim(),
                Stock = dTO.stock,
                Precio = dTO.precio,
                ProveedorId = dTO.proveedorId
            });
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id)
        {
            if (await repo.HasDependentsAsync(id))
                return (false, "No se puede eliminar el medicamento porque tiene registros asociados");
            var deleted = await repo.DeleteAsync(id);
            return deleted ? (true, null) : (false, "Medicamento no encontrado");
        }

        // Lista para llenar los combos de otros formularios
        public async Task<IReadOnlyList<MedicamentoTableDTO>> GetAllAsync()
        {
            var lista = await repo.GetAllAsync();
            return lista.Select(c => new MedicamentoTableDTO
            {
                id = c.Id,
                nombre = c.Nombre,
                descripcion = c.Descripcion,
                stock = c.Stock,
                precio = c.Precio,
                proveedorId = c.ProveedorId,
                proveedorName = c.Proveedor.Nombre,
                display = c.Nombre
            }).ToList();
        }

        public async Task<SaveMedicamentoDTO?> GetByidAsync(int id)
        {
            var medicamento = await repo.GetByIdAsync(id);
            if (medicamento == null)
                return null;
            return new SaveMedicamentoDTO
            {
                Id = medicamento.Id,
                nombre = medicamento.Nombre,
                descripcion = medicamento.Descripcion,
                stock = medicamento.Stock,
                precio = medicamento.Precio,
                proveedorId = medicamento.ProveedorId
            };
        }

        public Task<datatableResponseDTO<MedicamentoTableDTO>> GetPagedAsync(DataTableRequestDTO request)
            => repo.GetPagedAsync(request);

        public async Task<(bool Success, string? ErrorMessage)> UpDateAsync(SaveMedicamentoDTO dTO)
        {
            if (!dTO.Id.HasValue)
                return (false, "id invalido");
            var medicamento = await repo.GetByIdAsync(dTO.Id.Value);
            if (medicamento == null)
                return (false, "Medicamento no encontrado");
            if (await repo.NombreExisteAsync(dTO.nombre.Trim(), dTO.Id))
                return (false, "Ya existe un medicamento con ese nombre");
            if (await repoProveedor.GetByIdAsync(dTO.proveedorId) == null)
                return (false, "El proveedor seleccionado no existe");
            medicamento.Nombre = dTO.nombre.Trim();
            medicamento.Descripcion = dTO.descripcion.Trim();
            medicamento.Stock = dTO.stock;
            medicamento.Precio = dTO.precio;
            medicamento.ProveedorId = dTO.proveedorId;
            await repo.UpdateAsync(medicamento);
            return (true, null);
        }
    }
}
