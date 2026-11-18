using Microsoft.EntityFrameworkCore;
using Hospital.Data;
using Hospital.DTOS;
using Hospital.Models;

namespace Hospital.Repository
{
    // Acceso a datos con EF Core: Consulta médica
    public class ConsultaMedicaRepository(AppDBcontext context) : IConsultaMedicaRepository
    {
        public async Task<ConsultaMedica> CreateAsync(ConsultaMedica consultaMedica)
        {
            var entry = await context.consultasmedicas.AddAsync(consultaMedica);
            await context.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var consultaMedica = await context.consultasmedicas.FindAsync(id);
            if (consultaMedica == null)
                return false;
            context.consultasmedicas.Remove(consultaMedica);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<IReadOnlyList<ConsultaMedica>> GetAllAsync()
        {
            return await context.consultasmedicas.Include(x => x.Doctor).Include(x => x.Cita).Include(x => x.Paciente).OrderBy(x => x.Fecha).ToListAsync();
        }

        public async Task<ConsultaMedica?> GetByIdAsync(int id)
        {
            return await context.consultasmedicas.FindAsync(id);
        }

        public async Task<datatableResponseDTO<ConsultaMedicaTableDTO>> GetPagedAsync(DataTableRequestDTO request)
        {
            var query = context.consultasmedicas.AsQueryable();
            // Se esperan secuencialmente: un mismo DbContext no admite consultas simultaneas
            var recordsTotal = await query.CountAsync();

            var search = request.search?.value?.Trim();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.Diagnostico.Contains(search)
                    || c.Tratamiento.Contains(search)
                    || ("Dr. " + c.Doctor.Nombre + " " + c.Doctor.Apellido).Contains(search)
                    || (c.Cita.Paciente.Nombre + " " + c.Cita.Paciente.Apellido + " - " + c.Cita.Motivo).Contains(search)
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
                "diagnostico" => desc ? query.OrderByDescending(c => c.Diagnostico) : query.OrderBy(c => c.Diagnostico),
                "tratamiento" => desc ? query.OrderByDescending(c => c.Tratamiento) : query.OrderBy(c => c.Tratamiento),
                "doctorname" => desc ? query.OrderByDescending(c => "Dr. " + c.Doctor.Nombre + " " + c.Doctor.Apellido) : query.OrderBy(c => "Dr. " + c.Doctor.Nombre + " " + c.Doctor.Apellido),
                "citaname" => desc ? query.OrderByDescending(c => c.Cita.Paciente.Nombre + " " + c.Cita.Paciente.Apellido + " - " + c.Cita.Motivo) : query.OrderBy(c => c.Cita.Paciente.Nombre + " " + c.Cita.Paciente.Apellido + " - " + c.Cita.Motivo),
                "pacientename" => desc ? query.OrderByDescending(c => c.Paciente.Nombre + " " + c.Paciente.Apellido) : query.OrderBy(c => c.Paciente.Nombre + " " + c.Paciente.Apellido),
                _ => desc ? query.OrderByDescending(c => c.Fecha) : query.OrderBy(c => c.Fecha)
            };

            var data = await query
                .Skip(request.start)
                .Take(request.length > 0 ? request.length : 10)
                .Select(c => new ConsultaMedicaTableDTO
                {
                    id = c.Id,
                    fecha = c.Fecha,
                    diagnostico = c.Diagnostico,
                    tratamiento = c.Tratamiento,
                    doctorId = c.DoctorId,
                    doctorName = "Dr. " + c.Doctor.Nombre + " " + c.Doctor.Apellido,
                    citaId = c.CitaId,
                    citaName = c.Cita.Paciente.Nombre + " " + c.Cita.Paciente.Apellido + " - " + c.Cita.Motivo,
                    pacienteId = c.PacienteId,
                    pacienteName = c.Paciente.Nombre + " " + c.Paciente.Apellido,
                    display = c.Paciente.Nombre + " " + c.Paciente.Apellido + " - " + c.Diagnostico
                })
                .ToListAsync();

            return new datatableResponseDTO<ConsultaMedicaTableDTO>
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
            return await context.examenesmedicos.AnyAsync(x => x.ConsultaMedicaId == id);
        }

        public async Task<bool> UpdateAsync(ConsultaMedica consultaMedica)
        {
            context.consultasmedicas.Update(consultaMedica);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
