using Microsoft.EntityFrameworkCore;
using Hospital.Data;
using Hospital.DTOS;
using Hospital.Models;

namespace Hospital.Repository
{
    // Acceso a datos con EF Core: Cirugía
    public class CirugiaRepository(AppDBcontext context) : ICirugiaRepository
    {
        public async Task<Cirugia> CreateAsync(Cirugia cirugia)
        {
            var entry = await context.cirugias.AddAsync(cirugia);
            await context.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var cirugia = await context.cirugias.FindAsync(id);
            if (cirugia == null)
                return false;
            context.cirugias.Remove(cirugia);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<Cirugia?> GetByIdAsync(int id)
        {
            return await context.cirugias.FindAsync(id);
        }

        public async Task<datatableResponseDTO<CirugiaTableDTO>> GetPagedAsync(DataTableRequestDTO request)
        {
            var query = context.cirugias.AsQueryable();
            // Se esperan secuencialmente: un mismo DbContext no admite consultas simultaneas
            var recordsTotal = await query.CountAsync();

            var search = request.search?.value?.Trim();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.Nombre.Contains(search)
                    || c.Estado.Contains(search)
                    || ("Dr. " + c.Doctor.Nombre + " " + c.Doctor.Apellido).Contains(search)
                    || ("Hab. " + c.Habitacion.Numero).Contains(search)
                    || (c.Paciente.Nombre + " " + c.Paciente.Apellido).Contains(search));
            }
            var recordsFiltered = await query.CountAsync();

            var order = request.order.FirstOrDefault();
            var desc = string.Equals(order?.dir, "desc", StringComparison.OrdinalIgnoreCase);
            var columnName = request.columns.ElementAtOrDefault(order?.column ?? 0)?.data ?? "fecha";

            query = columnName.ToLowerInvariant() switch
            {
                "id" => desc ? query.OrderByDescending(c => c.Id) : query.OrderBy(c => c.Id),
                "nombre" => desc ? query.OrderByDescending(c => c.Nombre) : query.OrderBy(c => c.Nombre),
                "fecha" => desc ? query.OrderByDescending(c => c.Fecha) : query.OrderBy(c => c.Fecha),
                "estado" => desc ? query.OrderByDescending(c => c.Estado) : query.OrderBy(c => c.Estado),
                "doctorname" => desc ? query.OrderByDescending(c => "Dr. " + c.Doctor.Nombre + " " + c.Doctor.Apellido) : query.OrderBy(c => "Dr. " + c.Doctor.Nombre + " " + c.Doctor.Apellido),
                "habitacionname" => desc ? query.OrderByDescending(c => "Hab. " + c.Habitacion.Numero) : query.OrderBy(c => "Hab. " + c.Habitacion.Numero),
                "pacientename" => desc ? query.OrderByDescending(c => c.Paciente.Nombre + " " + c.Paciente.Apellido) : query.OrderBy(c => c.Paciente.Nombre + " " + c.Paciente.Apellido),
                _ => desc ? query.OrderByDescending(c => c.Fecha) : query.OrderBy(c => c.Fecha)
            };

            var data = await query
                .Skip(request.start)
                .Take(request.length > 0 ? request.length : 10)
                .Select(c => new CirugiaTableDTO
                {
                    id = c.Id,
                    nombre = c.Nombre,
                    fecha = c.Fecha,
                    estado = c.Estado,
                    doctorId = c.DoctorId,
                    doctorName = "Dr. " + c.Doctor.Nombre + " " + c.Doctor.Apellido,
                    habitacionId = c.HabitacionId,
                    habitacionName = "Hab. " + c.Habitacion.Numero,
                    pacienteId = c.PacienteId,
                    pacienteName = c.Paciente.Nombre + " " + c.Paciente.Apellido
                })
                .ToListAsync();

            return new datatableResponseDTO<CirugiaTableDTO>
            {
                draw = request.draw,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsFiltered,
                data = data
            };
        }

        // Verifica que la habitacion no este reservada para otra cirugia a la misma hora
        public Task<bool> HabitacionOcupadaAsync(int habitacionId, DateTime fecha, int? excludeId = null)
        {
            return context.cirugias.AnyAsync(x => x.HabitacionId == habitacionId && x.Fecha == fecha
                && x.Estado != "Cancelada" && (!excludeId.HasValue || x.Id != excludeId.Value));
        }

        public async Task<bool> UpdateAsync(Cirugia cirugia)
        {
            context.cirugias.Update(cirugia);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
