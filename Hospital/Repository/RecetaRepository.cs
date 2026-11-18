using Microsoft.EntityFrameworkCore;
using Hospital.Data;
using Hospital.DTOS;
using Hospital.Models;

namespace Hospital.Repository
{
    // Acceso a datos con EF Core: Receta
    public class RecetaRepository(AppDBcontext context) : IRecetaRepository
    {
        public async Task<Receta> CreateAsync(Receta receta)
        {
            var entry = await context.recetas.AddAsync(receta);
            await context.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<Receta?> GetByIdAsync(int id)
        {
            return await context.recetas.FindAsync(id);
        }

        public async Task<IReadOnlyList<Receta>> GetByPacienteAsync(int pacienteId)
        {
            return await context.recetas.Include(x => x.Doctor)
                .Where(x => x.PacienteId == pacienteId)
                .OrderByDescending(x => x.FechaSolicitud).ToListAsync();
        }

        public async Task<datatableResponseDTO<RecetaTableDTO>> GetPagedAsync(DataTableRequestDTO request)
        {
            var query = context.recetas.AsQueryable();
            var recordsTotal = await query.CountAsync();

            var search = request.search?.value?.Trim();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(r => r.Estado.Contains(search)
                    || r.Medicamento.Contains(search)
                    || r.Paciente.Cedula.Contains(search)
                    || (r.Paciente.Nombre + " " + r.Paciente.Apellido).Contains(search)
                    || ("Dr. " + r.Doctor.Nombre + " " + r.Doctor.Apellido).Contains(search));
            }
            var recordsFiltered = await query.CountAsync();

            var order = request.order.FirstOrDefault();
            var desc = string.Equals(order?.dir, "desc", StringComparison.OrdinalIgnoreCase);
            var columnName = request.columns.ElementAtOrDefault(order?.column ?? 0)?.data ?? "fechaSolicitud";

            query = columnName.ToLowerInvariant() switch
            {
                "id" => desc ? query.OrderByDescending(r => r.Id) : query.OrderBy(r => r.Id),
                "medicamento" => desc ? query.OrderByDescending(r => r.Medicamento) : query.OrderBy(r => r.Medicamento),
                "estado" => desc ? query.OrderByDescending(r => r.Estado) : query.OrderBy(r => r.Estado),
                "pacientename" => desc ? query.OrderByDescending(r => r.Paciente.Nombre + " " + r.Paciente.Apellido) : query.OrderBy(r => r.Paciente.Nombre + " " + r.Paciente.Apellido),
                "doctorname" => desc ? query.OrderByDescending(r => r.Doctor.Nombre + " " + r.Doctor.Apellido) : query.OrderBy(r => r.Doctor.Nombre + " " + r.Doctor.Apellido),
                _ => desc ? query.OrderByDescending(r => r.FechaSolicitud) : query.OrderBy(r => r.FechaSolicitud)
            };

            var data = await query
                .Skip(request.start)
                .Take(request.length > 0 ? request.length : 10)
                .Select(r => new RecetaTableDTO
                {
                    id = r.Id,
                    fechaSolicitud = r.FechaSolicitud,
                    medicamento = r.Medicamento,
                    indicaciones = r.Indicaciones,
                    estado = r.Estado,
                    respuesta = r.Respuesta,
                    doctorId = r.DoctorId,
                    doctorName = "Dr. " + r.Doctor.Nombre + " " + r.Doctor.Apellido,
                    pacienteId = r.PacienteId,
                    pacienteName = r.Paciente.Nombre + " " + r.Paciente.Apellido,
                    pacienteCedula = r.Paciente.Cedula
                })
                .ToListAsync();

            return new datatableResponseDTO<RecetaTableDTO>
            {
                draw = request.draw,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsFiltered,
                data = data
            };
        }

        public async Task<bool> UpdateAsync(Receta receta)
        {
            context.recetas.Update(receta);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
