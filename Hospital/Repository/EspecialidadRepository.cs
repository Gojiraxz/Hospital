using Microsoft.EntityFrameworkCore;
using Hospital.Data;
using Hospital.DTOS;
using Hospital.Models;

namespace Hospital.Repository
{
    // Acceso a datos con EF Core: Especialidad
    public class EspecialidadRepository(AppDBcontext context) : IEspecialidadRepository
    {
        public async Task<Especialidad> CreateAsync(Especialidad especialidad)
        {
            var entry = await context.especialidades.AddAsync(especialidad);
            await context.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var especialidad = await context.especialidades.FindAsync(id);
            if (especialidad == null)
                return false;
            context.especialidades.Remove(especialidad);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<IReadOnlyList<Especialidad>> GetAllAsync()
        {
            return await context.especialidades.OrderBy(x => x.Nombre).ToListAsync();
        }

        public async Task<Especialidad?> GetByIdAsync(int id)
        {
            return await context.especialidades.FindAsync(id);
        }

        public async Task<datatableResponseDTO<EspecialidadTableDTO>> GetPagedAsync(DataTableRequestDTO request)
        {
            var query = context.especialidades.AsQueryable();
            // Se esperan secuencialmente: un mismo DbContext no admite consultas simultaneas
            var recordsTotal = await query.CountAsync();

            var search = request.search?.value?.Trim();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.Nombre.Contains(search));
            }
            var recordsFiltered = await query.CountAsync();

            var order = request.order.FirstOrDefault();
            var desc = string.Equals(order?.dir, "desc", StringComparison.OrdinalIgnoreCase);
            var columnName = request.columns.ElementAtOrDefault(order?.column ?? 0)?.data ?? "nombre";

            query = columnName.ToLowerInvariant() switch
            {
                "id" => desc ? query.OrderByDescending(c => c.Id) : query.OrderBy(c => c.Id),
                "nombre" => desc ? query.OrderByDescending(c => c.Nombre) : query.OrderBy(c => c.Nombre),
                _ => desc ? query.OrderByDescending(c => c.Nombre) : query.OrderBy(c => c.Nombre)
            };

            var data = await query
                .Skip(request.start)
                .Take(request.length > 0 ? request.length : 10)
                .Select(c => new EspecialidadTableDTO
                {
                    id = c.Id,
                    nombre = c.Nombre,
                    display = c.Nombre
                })
                .ToListAsync();

            return new datatableResponseDTO<EspecialidadTableDTO>
            {
                draw = request.draw,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsFiltered,
                data = data
            };
        }

        public async Task<bool> NombreExisteAsync(string valor, int? excludeId = null)
        {
            return await context.especialidades.AnyAsync(x => x.Nombre == valor && (!excludeId.HasValue || x.Id != excludeId.Value));
        }

        // Evita borrar registros que ya tienen datos relacionados
        public async Task<bool> HasDependentsAsync(int id)
        {
            return await context.doctores.AnyAsync(x => x.EspecialidadId == id) || await context.enfermeros.AnyAsync(x => x.EspecialidadId == id);
        }

        public async Task<bool> UpdateAsync(Especialidad especialidad)
        {
            context.especialidades.Update(especialidad);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
