using Microsoft.EntityFrameworkCore;
using Hospital.Data;
using Hospital.DTOS;
using Hospital.Models;

namespace Hospital.Repository
{
    // Acceso a datos con EF Core: Paciente
    public class PacienteRepository(AppDBcontext context) : IPacienteRepository
    {
        public async Task<Paciente> CreateAsync(Paciente paciente)
        {
            var entry = await context.pacientes.AddAsync(paciente);
            await context.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var paciente = await context.pacientes.FindAsync(id);
            if (paciente == null)
                return false;
            context.pacientes.Remove(paciente);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<IReadOnlyList<Paciente>> GetAllAsync()
        {
            return await context.pacientes.OrderBy(x => x.Apellido).ToListAsync();
        }

        public async Task<Paciente?> GetByIdAsync(int id)
        {
            return await context.pacientes.FindAsync(id);
        }

        public async Task<datatableResponseDTO<PacienteTableDTO>> GetPagedAsync(DataTableRequestDTO request)
        {
            var query = context.pacientes.AsQueryable();
            // Se esperan secuencialmente: un mismo DbContext no admite consultas simultaneas
            var recordsTotal = await query.CountAsync();

            var search = request.search?.value?.Trim();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.Nombre.Contains(search)
                    || c.Apellido.Contains(search)
                    || c.Cedula.Contains(search)
                    || c.Sexo.Contains(search)
                    || c.Telefono.Contains(search)
                    || c.Direccion.Contains(search));
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
                "cedula" => desc ? query.OrderByDescending(c => c.Cedula) : query.OrderBy(c => c.Cedula),
                "fechanacimiento" => desc ? query.OrderByDescending(c => c.FechaNacimiento) : query.OrderBy(c => c.FechaNacimiento),
                "sexo" => desc ? query.OrderByDescending(c => c.Sexo) : query.OrderBy(c => c.Sexo),
                "telefono" => desc ? query.OrderByDescending(c => c.Telefono) : query.OrderBy(c => c.Telefono),
                "direccion" => desc ? query.OrderByDescending(c => c.Direccion) : query.OrderBy(c => c.Direccion),
                _ => desc ? query.OrderByDescending(c => c.Apellido) : query.OrderBy(c => c.Apellido)
            };

            var data = await query
                .Skip(request.start)
                .Take(request.length > 0 ? request.length : 10)
                .Select(c => new PacienteTableDTO
                {
                    id = c.Id,
                    nombre = c.Nombre,
                    apellido = c.Apellido,
                    cedula = c.Cedula,
                    fechaNacimiento = c.FechaNacimiento,
                    sexo = c.Sexo,
                    telefono = c.Telefono,
                    direccion = c.Direccion,
                    display = c.Nombre + " " + c.Apellido
                })
                .ToListAsync();

            return new datatableResponseDTO<PacienteTableDTO>
            {
                draw = request.draw,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsFiltered,
                data = data
            };
        }

        // Busca por cedula ignorando guiones y espacios (001-150390-0001A == 0011503900001A)
        public async Task<Paciente?> GetByCedulaAsync(string cedula)
        {
            var normalizada = (cedula ?? string.Empty).Replace("-", "").Replace(" ", "").Trim();
            if (normalizada.Length == 0)
                return null;
            return await context.pacientes
                .FirstOrDefaultAsync(x => x.Cedula.Replace("-", "").Replace(" ", "") == normalizada);
        }

        public async Task<bool> CedulaExisteAsync(string valor, int? excludeId = null)
        {
            return await context.pacientes.AnyAsync(x => x.Cedula == valor && (!excludeId.HasValue || x.Id != excludeId.Value));
        }

        // Evita borrar registros que ya tienen datos relacionados
        public async Task<bool> HasDependentsAsync(int id)
        {
            return await context.citas.AnyAsync(x => x.PacienteId == id) || await context.recetas.AnyAsync(x => x.PacienteId == id) || await context.habitaciones.AnyAsync(x => x.PacienteId == id) || await context.consultasmedicas.AnyAsync(x => x.PacienteId == id) || await context.cirugias.AnyAsync(x => x.PacienteId == id) || await context.facturas.AnyAsync(x => x.PacienteId == id);
        }

        public async Task<bool> UpdateAsync(Paciente paciente)
        {
            context.pacientes.Update(paciente);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
