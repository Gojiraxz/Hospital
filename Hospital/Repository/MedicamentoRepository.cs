using Microsoft.EntityFrameworkCore;
using Hospital.Data;
using Hospital.DTOS;
using Hospital.Models;

namespace Hospital.Repository
{
    // Acceso a datos con EF Core: Medicamento
    public class MedicamentoRepository(AppDBcontext context) : IMedicamentoRepository
    {
        public async Task<Medicamento> CreateAsync(Medicamento medicamento)
        {
            var entry = await context.medicamentos.AddAsync(medicamento);
            await context.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var medicamento = await context.medicamentos.FindAsync(id);
            if (medicamento == null)
                return false;
            context.medicamentos.Remove(medicamento);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<IReadOnlyList<Medicamento>> GetAllAsync()
        {
            return await context.medicamentos.Include(x => x.Proveedor).OrderBy(x => x.Nombre).ToListAsync();
        }

        public async Task<Medicamento?> GetByIdAsync(int id)
        {
            return await context.medicamentos.FindAsync(id);
        }

        public async Task<datatableResponseDTO<MedicamentoTableDTO>> GetPagedAsync(DataTableRequestDTO request)
        {
            var query = context.medicamentos.AsQueryable();
            // Se esperan secuencialmente: un mismo DbContext no admite consultas simultaneas
            var recordsTotal = await query.CountAsync();

            var search = request.search?.value?.Trim();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.Nombre.Contains(search)
                    || c.Descripcion.Contains(search)
                    || (c.Proveedor.Nombre).Contains(search));
            }
            var recordsFiltered = await query.CountAsync();

            var order = request.order.FirstOrDefault();
            var desc = string.Equals(order?.dir, "desc", StringComparison.OrdinalIgnoreCase);
            var columnName = request.columns.ElementAtOrDefault(order?.column ?? 0)?.data ?? "nombre";

            query = columnName.ToLowerInvariant() switch
            {
                "id" => desc ? query.OrderByDescending(c => c.Id) : query.OrderBy(c => c.Id),
                "nombre" => desc ? query.OrderByDescending(c => c.Nombre) : query.OrderBy(c => c.Nombre),
                "descripcion" => desc ? query.OrderByDescending(c => c.Descripcion) : query.OrderBy(c => c.Descripcion),
                "stock" => desc ? query.OrderByDescending(c => c.Stock) : query.OrderBy(c => c.Stock),
                "precio" => desc ? query.OrderByDescending(c => c.Precio) : query.OrderBy(c => c.Precio),
                "proveedorname" => desc ? query.OrderByDescending(c => c.Proveedor.Nombre) : query.OrderBy(c => c.Proveedor.Nombre),
                _ => desc ? query.OrderByDescending(c => c.Nombre) : query.OrderBy(c => c.Nombre)
            };

            var data = await query
                .Skip(request.start)
                .Take(request.length > 0 ? request.length : 10)
                .Select(c => new MedicamentoTableDTO
                {
                    id = c.Id,
                    nombre = c.Nombre,
                    descripcion = c.Descripcion,
                    stock = c.Stock,
                    precio = c.Precio,
                    proveedorId = c.ProveedorId,
                    proveedorName = c.Proveedor.Nombre,
                    display = c.Nombre
                })
                .ToListAsync();

            return new datatableResponseDTO<MedicamentoTableDTO>
            {
                draw = request.draw,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsFiltered,
                data = data
            };
        }

        public async Task<bool> NombreExisteAsync(string valor, int? excludeId = null)
        {
            return await context.medicamentos.AnyAsync(x => x.Nombre == valor && (!excludeId.HasValue || x.Id != excludeId.Value));
        }

        // Evita borrar registros que ya tienen datos relacionados
        public async Task<bool> HasDependentsAsync(int id)
        {
            return await context.detallesfactura.AnyAsync(x => x.MedicamentoId == id);
        }

        public async Task<bool> UpdateAsync(Medicamento medicamento)
        {
            context.medicamentos.Update(medicamento);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
