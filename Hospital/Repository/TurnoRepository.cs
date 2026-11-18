using Microsoft.EntityFrameworkCore;
using Hospital.Data;
using Hospital.DTOS;
using Hospital.Models;

namespace Hospital.Repository
{
    // Acceso a datos con EF Core: Turno
    public class TurnoRepository(AppDBcontext context) : ITurnoRepository
    {
        public async Task<Turno> CreateAsync(Turno turno)
        {
            var entry = await context.turnos.AddAsync(turno);
            await context.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var turno = await context.turnos.FindAsync(id);
            if (turno == null)
                return false;
            context.turnos.Remove(turno);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<Turno?> GetByIdAsync(int id)
        {
            return await context.turnos.FindAsync(id);
        }

        public async Task<datatableResponseDTO<TurnoTableDTO>> GetPagedAsync(DataTableRequestDTO request)
        {
            var query = context.turnos.AsQueryable();
            // Se esperan secuencialmente: un mismo DbContext no admite consultas simultaneas
            var recordsTotal = await query.CountAsync();

            var search = request.search?.value?.Trim();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.Inicio.Contains(search)
                    || c.Final.Contains(search)
                    || (c.Enfermero.Nombre + " " + c.Enfermero.Apellido).Contains(search)
                    || ("Dr. " + c.Doctor.Nombre + " " + c.Doctor.Apellido).Contains(search));
            }
            var recordsFiltered = await query.CountAsync();

            var order = request.order.FirstOrDefault();
            var desc = string.Equals(order?.dir, "desc", StringComparison.OrdinalIgnoreCase);
            var columnName = request.columns.ElementAtOrDefault(order?.column ?? 0)?.data ?? "inicio";

            query = columnName.ToLowerInvariant() switch
            {
                "id" => desc ? query.OrderByDescending(c => c.Id) : query.OrderBy(c => c.Id),
                "inicio" => desc ? query.OrderByDescending(c => c.Inicio) : query.OrderBy(c => c.Inicio),
                "final" => desc ? query.OrderByDescending(c => c.Final) : query.OrderBy(c => c.Final),
                "enfermeroname" => desc ? query.OrderByDescending(c => c.Enfermero.Nombre + " " + c.Enfermero.Apellido) : query.OrderBy(c => c.Enfermero.Nombre + " " + c.Enfermero.Apellido),
                "doctorname" => desc ? query.OrderByDescending(c => "Dr. " + c.Doctor.Nombre + " " + c.Doctor.Apellido) : query.OrderBy(c => "Dr. " + c.Doctor.Nombre + " " + c.Doctor.Apellido),
                _ => desc ? query.OrderByDescending(c => c.Inicio) : query.OrderBy(c => c.Inicio)
            };

            var data = await query
                .Skip(request.start)
                .Take(request.length > 0 ? request.length : 10)
                .Select(c => new TurnoTableDTO
                {
                    id = c.Id,
                    inicio = c.Inicio,
                    final = c.Final,
                    enfermeroId = c.EnfermeroId,
                    enfermeroName = c.Enfermero.Nombre + " " + c.Enfermero.Apellido,
                    doctorId = c.DoctorId,
                    doctorName = "Dr. " + c.Doctor.Nombre + " " + c.Doctor.Apellido
                })
                .ToListAsync();

            return new datatableResponseDTO<TurnoTableDTO>
            {
                draw = request.draw,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsFiltered,
                data = data
            };
        }

        public async Task<bool> UpdateAsync(Turno turno)
        {
            context.turnos.Update(turno);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
