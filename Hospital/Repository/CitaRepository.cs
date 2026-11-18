using Microsoft.EntityFrameworkCore;
using Hospital.Data;
using Hospital.DTOS;
using Hospital.Models;

namespace Hospital.Repository
{
    // Acceso a datos con EF Core: Cita
    public class CitaRepository(AppDBcontext context) : ICitaRepository
    {
        public async Task<Cita> CreateAsync(Cita cita)
        {
            var entry = await context.citas.AddAsync(cita);
            await context.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var cita = await context.citas.FindAsync(id);
            if (cita == null)
                return false;
            context.citas.Remove(cita);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<IReadOnlyList<Cita>> GetAllAsync()
        {
            return await context.citas.Include(x => x.Doctor).Include(x => x.Paciente).OrderBy(x => x.Fecha).ToListAsync();
        }

        public async Task<IReadOnlyList<Cita>> GetByPacienteAsync(int pacienteId)
        {
            return await context.citas.Include(x => x.Doctor)
                .Where(x => x.PacienteId == pacienteId)
                .OrderByDescending(x => x.Fecha).ToListAsync();
        }

        public async Task<Cita?> GetByIdAsync(int id)
        {
            return await context.citas.FindAsync(id);
        }

        public async Task<datatableResponseDTO<CitaTableDTO>> GetPagedAsync(DataTableRequestDTO request)
        {
            var query = context.citas.AsQueryable();
            // Se esperan secuencialmente: un mismo DbContext no admite consultas simultaneas
            var recordsTotal = await query.CountAsync();

            var search = request.search?.value?.Trim();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.Estado.Contains(search)
                    || c.Motivo.Contains(search)
                    || ("Dr. " + c.Doctor.Nombre + " " + c.Doctor.Apellido).Contains(search)
                    || (c.Paciente.Nombre + " " + c.Paciente.Apellido).Contains(search));
            }
            var recordsFiltered = await query.CountAsync();

            var order = request.order.FirstOrDefault();
            var desc = string.Equals(order?.dir, "desc", StringComparison.OrdinalIgnoreCase);
            var columnName = request.columns.ElementAtOrDefault(order?.column ?? 0)?.data ?? "fecha";

            query = columnName.ToLowerInvariant() switch
            {
                "id" => desc ? query.OrderByDescending(c => c.Id) : query.OrderBy(c => c.Id),
                "fecha" => desc ? query.OrderByDescending(c => c.Fecha) : query.OrderBy(c => c.Fecha),
                "estado" => desc ? query.OrderByDescending(c => c.Estado) : query.OrderBy(c => c.Estado),
                "motivo" => desc ? query.OrderByDescending(c => c.Motivo) : query.OrderBy(c => c.Motivo),
                "doctorname" => desc ? query.OrderByDescending(c => "Dr. " + c.Doctor.Nombre + " " + c.Doctor.Apellido) : query.OrderBy(c => "Dr. " + c.Doctor.Nombre + " " + c.Doctor.Apellido),
                "pacientename" => desc ? query.OrderByDescending(c => c.Paciente.Nombre + " " + c.Paciente.Apellido) : query.OrderBy(c => c.Paciente.Nombre + " " + c.Paciente.Apellido),
                _ => desc ? query.OrderByDescending(c => c.Fecha) : query.OrderBy(c => c.Fecha)
            };

            var data = await query
                .Skip(request.start)
                .Take(request.length > 0 ? request.length : 10)
                .Select(c => new CitaTableDTO
                {
                    id = c.Id,
                    fecha = c.Fecha,
                    estado = c.Estado,
                    motivo = c.Motivo,
                    doctorId = c.DoctorId,
                    doctorName = "Dr. " + c.Doctor.Nombre + " " + c.Doctor.Apellido,
                    pacienteId = c.PacienteId,
                    pacienteName = c.Paciente.Nombre + " " + c.Paciente.Apellido,
                    display = c.Paciente.Nombre + " " + c.Paciente.Apellido + " - " + c.Motivo
                })
                .ToListAsync();

            return new datatableResponseDTO<CitaTableDTO>
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
            return await context.consultasmedicas.AnyAsync(x => x.CitaId == id);
        }

        // Verifica que el doctor no tenga otra cita a la misma hora
        public Task<bool> DoctorOcupadoAsync(int doctorId, DateTime fecha, int? excludeId = null)
        {
            return context.citas.AnyAsync(x => x.DoctorId == doctorId && x.Fecha == fecha
                && x.Estado != "Cancelada" && (!excludeId.HasValue || x.Id != excludeId.Value));
        }

        public async Task<bool> UpdateAsync(Cita cita)
        {
            context.citas.Update(cita);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
