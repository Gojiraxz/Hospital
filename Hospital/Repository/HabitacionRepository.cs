using Microsoft.EntityFrameworkCore;
using Hospital.Data;
using Hospital.DTOS;
using Hospital.Models;

namespace Hospital.Repository
{
    // Acceso a datos con EF Core: Habitación
    public class HabitacionRepository(AppDBcontext context) : IHabitacionRepository
    {
        public async Task<Habitacion> CreateAsync(Habitacion habitacion)
        {
            var entry = await context.habitaciones.AddAsync(habitacion);
            await context.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var habitacion = await context.habitaciones.FindAsync(id);
            if (habitacion == null)
                return false;
            context.habitaciones.Remove(habitacion);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<IReadOnlyList<Habitacion>> GetAllAsync()
        {
            return await context.habitaciones.Include(x => x.Paciente).OrderBy(x => x.Numero).ToListAsync();
        }

        public async Task<Habitacion?> GetByIdAsync(int id)
        {
            return await context.habitaciones.FindAsync(id);
        }

        public async Task<datatableResponseDTO<HabitacionTableDTO>> GetPagedAsync(DataTableRequestDTO request)
        {
            var query = context.habitaciones.AsQueryable();
            // Se esperan secuencialmente: un mismo DbContext no admite consultas simultaneas
            var recordsTotal = await query.CountAsync();

            var search = request.search?.value?.Trim();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.Numero.Contains(search)
                    || c.Estado.Contains(search)
                    || (c.Paciente != null && (c.Paciente.Nombre + " " + c.Paciente.Apellido).Contains(search)));
            }
            var recordsFiltered = await query.CountAsync();

            var order = request.order.FirstOrDefault();
            var desc = string.Equals(order?.dir, "desc", StringComparison.OrdinalIgnoreCase);
            var columnName = request.columns.ElementAtOrDefault(order?.column ?? 0)?.data ?? "numero";

            query = columnName.ToLowerInvariant() switch
            {
                "id" => desc ? query.OrderByDescending(c => c.Id) : query.OrderBy(c => c.Id),
                "numero" => desc ? query.OrderByDescending(c => c.Numero) : query.OrderBy(c => c.Numero),
                "estado" => desc ? query.OrderByDescending(c => c.Estado) : query.OrderBy(c => c.Estado),
                "pacientename" => desc ? query.OrderByDescending(c => c.Paciente != null ? c.Paciente.Nombre + " " + c.Paciente.Apellido : "") : query.OrderBy(c => c.Paciente != null ? c.Paciente.Nombre + " " + c.Paciente.Apellido : ""),
                _ => desc ? query.OrderByDescending(c => c.Numero) : query.OrderBy(c => c.Numero)
            };

            var data = await query
                .Skip(request.start)
                .Take(request.length > 0 ? request.length : 10)
                .Select(c => new HabitacionTableDTO
                {
                    id = c.Id,
                    numero = c.Numero,
                    estado = c.Estado,
                    pacienteId = c.PacienteId,
                    pacienteName = c.Paciente != null ? c.Paciente.Nombre + " " + c.Paciente.Apellido : "",
                    display = "Hab. " + c.Numero
                })
                .ToListAsync();

            return new datatableResponseDTO<HabitacionTableDTO>
            {
                draw = request.draw,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsFiltered,
                data = data
            };
        }

        // Un paciente solo puede estar en una habitacion a la vez
        public async Task<bool> PacienteEnOtraHabitacionAsync(int pacienteId, int? excludeId = null)
        {
            return await context.habitaciones.AnyAsync(x => x.PacienteId == pacienteId && (!excludeId.HasValue || x.Id != excludeId.Value));
        }

        public async Task<bool> NumeroExisteAsync(string valor, int? excludeId = null)
        {
            return await context.habitaciones.AnyAsync(x => x.Numero == valor && (!excludeId.HasValue || x.Id != excludeId.Value));
        }

        // Evita borrar registros que ya tienen datos relacionados
        public async Task<bool> HasDependentsAsync(int id)
        {
            return await context.cirugias.AnyAsync(x => x.HabitacionId == id);
        }

        public async Task<bool> UpdateAsync(Habitacion habitacion)
        {
            context.habitaciones.Update(habitacion);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
