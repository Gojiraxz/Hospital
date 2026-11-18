using Microsoft.EntityFrameworkCore;
using Hospital.Data;
using Hospital.DTOS;
using Hospital.Models;

namespace Hospital.Repository
{
    // Acceso a datos con EF Core: Laboratorio
    public class LaboratorioRepository(AppDBcontext context) : ILaboratorioRepository
    {
        public async Task<Laboratorio> CreateAsync(Laboratorio laboratorio)
        {
            var entry = await context.laboratorios.AddAsync(laboratorio);
            await context.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var laboratorio = await context.laboratorios.FindAsync(id);
            if (laboratorio == null)
                return false;
            context.laboratorios.Remove(laboratorio);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<IReadOnlyList<Laboratorio>> GetAllAsync()
        {
            return await context.laboratorios.OrderBy(x => x.Nombre).ToListAsync();
        }

        public async Task<Laboratorio?> GetByIdAsync(int id)
        {
            return await context.laboratorios.FindAsync(id);
        }

        public async Task<datatableResponseDTO<LaboratorioTableDTO>> GetPagedAsync(DataTableRequestDTO request)
        {
            var query = context.laboratorios.AsQueryable();
            // Se esperan secuencialmente: un mismo DbContext no admite consultas simultaneas
            var recordsTotal = await query.CountAsync();

            var search = request.search?.value?.Trim();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.Nombre.Contains(search)
                    || c.Ubicacion.Contains(search)
                    || c.Telefono.Contains(search));
            }
            var recordsFiltered = await query.CountAsync();

            var order = request.order.FirstOrDefault();
            var desc = string.Equals(order?.dir, "desc", StringComparison.OrdinalIgnoreCase);
            var columnName = request.columns.ElementAtOrDefault(order?.column ?? 0)?.data ?? "nombre";

            query = columnName.ToLowerInvariant() switch
            {
                "id" => desc ? query.OrderByDescending(c => c.Id) : query.OrderBy(c => c.Id),
                "nombre" => desc ? query.OrderByDescending(c => c.Nombre) : query.OrderBy(c => c.Nombre),
                "ubicacion" => desc ? query.OrderByDescending(c => c.Ubicacion) : query.OrderBy(c => c.Ubicacion),
                "telefono" => desc ? query.OrderByDescending(c => c.Telefono) : query.OrderBy(c => c.Telefono),
                _ => desc ? query.OrderByDescending(c => c.Nombre) : query.OrderBy(c => c.Nombre)
            };

            var data = await query
                .Skip(request.start)
                .Take(request.length > 0 ? request.length : 10)
                .Select(c => new LaboratorioTableDTO
                {
                    id = c.Id,
                    nombre = c.Nombre,
                    ubicacion = c.Ubicacion,
                    telefono = c.Telefono,
                    display = c.Nombre
                })
                .ToListAsync();

            return new datatableResponseDTO<LaboratorioTableDTO>
            {
                draw = request.draw,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsFiltered,
                data = data
            };
        }

        public async Task<bool> NombreExisteAsync(string valor, int? excludeId = null)
        {
            return await context.laboratorios.AnyAsync(x => x.Nombre == valor && (!excludeId.HasValue || x.Id != excludeId.Value));
        }

        // Evita borrar registros que ya tienen datos relacionados
        public async Task<bool> HasDependentsAsync(int id)
        {
            return await context.examenesmedicos.AnyAsync(x => x.LaboratorioId == id);
        }

        public async Task<bool> UpdateAsync(Laboratorio laboratorio)
        {
            context.laboratorios.Update(laboratorio);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
