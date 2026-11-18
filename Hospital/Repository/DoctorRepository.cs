using Microsoft.EntityFrameworkCore;
using Hospital.Data;
using Hospital.DTOS;
using Hospital.Models;

namespace Hospital.Repository
{
    // Acceso a datos con EF Core: Doctor
    public class DoctorRepository(AppDBcontext context) : IDoctorRepository
    {
        public async Task<Doctor> CreateAsync(Doctor doctor)
        {
            var entry = await context.doctores.AddAsync(doctor);
            await context.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var doctor = await context.doctores.FindAsync(id);
            if (doctor == null)
                return false;
            context.doctores.Remove(doctor);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<IReadOnlyList<Doctor>> GetAllAsync()
        {
            return await context.doctores.Include(x => x.Especialidad).OrderBy(x => x.Apellido).ToListAsync();
        }

        public async Task<Doctor?> GetByIdAsync(int id)
        {
            return await context.doctores.FindAsync(id);
        }

        public async Task<datatableResponseDTO<DoctorTableDTO>> GetPagedAsync(DataTableRequestDTO request)
        {
            var query = context.doctores.AsQueryable();
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
                .Select(c => new DoctorTableDTO
                {
                    id = c.Id,
                    nombre = c.Nombre,
                    apellido = c.Apellido,
                    especialidadId = c.EspecialidadId,
                    especialidadName = c.Especialidad.Nombre,
                    display = "Dr. " + c.Nombre + " " + c.Apellido
                })
                .ToListAsync();

            return new datatableResponseDTO<DoctorTableDTO>
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
            return await context.turnos.AnyAsync(x => x.DoctorId == id) || await context.recetas.AnyAsync(x => x.DoctorId == id) || await context.citas.AnyAsync(x => x.DoctorId == id) || await context.consultasmedicas.AnyAsync(x => x.DoctorId == id) || await context.cirugias.AnyAsync(x => x.DoctorId == id);
        }

        public async Task<bool> UpdateAsync(Doctor doctor)
        {
            context.doctores.Update(doctor);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
