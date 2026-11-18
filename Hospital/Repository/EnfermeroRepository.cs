using Microsoft.EntityFrameworkCore;
using Hospital.Data;
using Hospital.DTOS;
using Hospital.Models;

namespace Hospital.Repository
{
    // Acceso a datos con EF Core: Enfermero
    public class EnfermeroRepository(AppDBcontext context) : IEnfermeroRepository
    {
        public async Task<Enfermero> CreateAsync(Enfermero enfermero)
        {
            var entry = await context.enfermeros.AddAsync(enfermero);
            await context.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var enfermero = await context.enfermeros.FindAsync(id);
            if (enfermero == null)
                return false;
            context.enfermeros.Remove(enfermero);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<IReadOnlyList<Enfermero>> GetAllAsync()
        {
            return await context.enfermeros.Include(x => x.Especialidad).OrderBy(x => x.Apellido).ToListAsync();
        }

        public async Task<Enfermero?> GetByIdAsync(int id)
        {
            return await context.enfermeros.FindAsync(id);
        }

        public async Task<datatableResponseDTO<EnfermeroTableDTO>> GetPagedAsync(DataTableRequestDTO request)
        {
            var query = context.enfermeros.AsQueryable();
            // Se esperan secuencialmente: un mismo DbContext no admite consultas simultaneas
            var recordsTotal = await query.CountAsync();

            var search = request.search?.value?.Trim();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.Nombre.Contains(search)
                    || c.Apellido.Contains(search)
                    || (c.Especialidad.Nombre).Contains(search));
            }
            var recordsFiltered = await query.CountAsync();

            var order = request.order.FirstOrDefault();
            var desc = string.Equals(order?.dir, "desc", StringComparison.OrdinalIgnoreCase);
            var columnName = request.columns.ElementAtOrDefault(order?.column ?? 0)?.data ?? "apellido";

            query = columnName.ToLowerInvariant() switch
            {
                "id" => desc ? query.OrderByDescending(c => c.Id) : query.OrderBy(c => c.Id),
                "nombre" => desc ? query.OrderByDescending(c => c.Nombre) : query.OrderBy(c => c.Nombre),
                "apellido" => desc ? query.OrderByDescending(c => c.Apellido) : query.OrderBy(c => c.Apellido),
                "especialidadname" => desc ? query.OrderByDescending(c => c.Especialidad.Nombre) : query.OrderBy(c => c.Especialidad.Nombre),
                _ => desc ? query.OrderByDescending(c => c.Apellido) : query.OrderBy(c => c.Apellido)
            };

            var data = await query
                .Skip(request.start)
                .Take(request.length > 0 ? request.length : 10)
                .Select(c => new EnfermeroTableDTO
                {
                    id = c.Id,
                    nombre = c.Nombre,
                    apellido = c.Apellido,
                    especialidadId = c.EspecialidadId,
                    especialidadName = c.Especialidad.Nombre,
                    display = c.Nombre + " " + c.Apellido
                })
                .ToListAsync();

            return new datatableResponseDTO<EnfermeroTableDTO>
            {
                draw = request.draw,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsFiltered,
                data = data
            };
        }

        // Evita borrar registros que ya tienen datos relacionados
        public async Task<bool> HasDependentsAsync(int id)
        {
            return await context.turnos.AnyAsync(x => x.EnfermeroId == id);
        }

        public async Task<bool> UpdateAsync(Enfermero enfermero)
        {
            context.enfermeros.Update(enfermero);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
