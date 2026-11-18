using Microsoft.EntityFrameworkCore;
using Hospital.Data;
using Hospital.DTOS;
using Hospital.Models;

namespace Hospital.Repository
{
    // Acceso a datos con EF Core: Proveedor
    public class ProveedorRepository(AppDBcontext context) : IProveedorRepository
    {
        public async Task<Proveedor> CreateAsync(Proveedor proveedor)
        {
            var entry = await context.proveedores.AddAsync(proveedor);
            await context.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var proveedor = await context.proveedores.FindAsync(id);
            if (proveedor == null)
                return false;
            context.proveedores.Remove(proveedor);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<IReadOnlyList<Proveedor>> GetAllAsync()
        {
            return await context.proveedores.OrderBy(x => x.Nombre).ToListAsync();
        }

        public async Task<Proveedor?> GetByIdAsync(int id)
        {
            return await context.proveedores.FindAsync(id);
        }

        public async Task<datatableResponseDTO<ProveedorTableDTO>> GetPagedAsync(DataTableRequestDTO request)
        {
            var query = context.proveedores.AsQueryable();
            // Se esperan secuencialmente: un mismo DbContext no admite consultas simultaneas
            var recordsTotal = await query.CountAsync();

            var search = request.search?.value?.Trim();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.Nombre.Contains(search)
                    || c.Correo.Contains(search)
                    || c.Cofepris.Contains(search));
            }
            var recordsFiltered = await query.CountAsync();

            var order = request.order.FirstOrDefault();
            var desc = string.Equals(order?.dir, "desc", StringComparison.OrdinalIgnoreCase);
            var columnName = request.columns.ElementAtOrDefault(order?.column ?? 0)?.data ?? "nombre";

            query = columnName.ToLowerInvariant() switch
            {
                "id" => desc ? query.OrderByDescending(c => c.Id) : query.OrderBy(c => c.Id),
                "nombre" => desc ? query.OrderByDescending(c => c.Nombre) : query.OrderBy(c => c.Nombre),
                "correo" => desc ? query.OrderByDescending(c => c.Correo) : query.OrderBy(c => c.Correo),
                "cofepris" => desc ? query.OrderByDescending(c => c.Cofepris) : query.OrderBy(c => c.Cofepris),
                _ => desc ? query.OrderByDescending(c => c.Nombre) : query.OrderBy(c => c.Nombre)
            };

            var data = await query
                .Skip(request.start)
                .Take(request.length > 0 ? request.length : 10)
                .Select(c => new ProveedorTableDTO
                {
                    id = c.Id,
                    nombre = c.Nombre,
                    correo = c.Correo,
                    cofepris = c.Cofepris,
                    display = c.Nombre
                })
                .ToListAsync();

            return new datatableResponseDTO<ProveedorTableDTO>
            {
                draw = request.draw,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsFiltered,
                data = data
            };
        }

        public async Task<bool> CofeprisExisteAsync(string valor, int? excludeId = null)
        {
            return await context.proveedores.AnyAsync(x => x.Cofepris == valor && (!excludeId.HasValue || x.Id != excludeId.Value));
        }

        // Evita borrar registros que ya tienen datos relacionados
        public async Task<bool> HasDependentsAsync(int id)
        {
            return await context.medicamentos.AnyAsync(x => x.ProveedorId == id);
        }

        public async Task<bool> UpdateAsync(Proveedor proveedor)
        {
            context.proveedores.Update(proveedor);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
