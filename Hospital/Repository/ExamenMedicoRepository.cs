using Microsoft.EntityFrameworkCore;
using Hospital.Data;
using Hospital.DTOS;
using Hospital.Models;

namespace Hospital.Repository
{
    // Acceso a datos con EF Core: Examen médico
    public class ExamenMedicoRepository(AppDBcontext context) : IExamenMedicoRepository
    {
        public async Task<ExamenMedico> CreateAsync(ExamenMedico examenMedico)
        {
            var entry = await context.examenesmedicos.AddAsync(examenMedico);
            await context.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var examenMedico = await context.examenesmedicos.FindAsync(id);
            if (examenMedico == null)
                return false;
            context.examenesmedicos.Remove(examenMedico);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<ExamenMedico?> GetByIdAsync(int id)
        {
            return await context.examenesmedicos.FindAsync(id);
        }

        public async Task<datatableResponseDTO<ExamenMedicoTableDTO>> GetPagedAsync(DataTableRequestDTO request)
        {
            var query = context.examenesmedicos.AsQueryable();
            // Se esperan secuencialmente: un mismo DbContext no admite consultas simultaneas
            var recordsTotal = await query.CountAsync();

            var search = request.search?.value?.Trim();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.TipoExamen.Contains(search)
                    || c.Resultado.Contains(search)
                    || c.Estado.Contains(search)
                    || (c.ConsultaMedica.Paciente.Nombre + " " + c.ConsultaMedica.Paciente.Apellido + " - " + c.ConsultaMedica.Diagnostico).Contains(search)
                    || (c.Laboratorio.Nombre).Contains(search));
            }
            var recordsFiltered = await query.CountAsync();

            var order = request.order.FirstOrDefault();
            var desc = string.Equals(order?.dir, "desc", StringComparison.OrdinalIgnoreCase);
            var columnName = request.columns.ElementAtOrDefault(order?.column ?? 0)?.data ?? "tipoExamen";

            query = columnName.ToLowerInvariant() switch
            {
                "id" => desc ? query.OrderByDescending(c => c.Id) : query.OrderBy(c => c.Id),
                "tipoexamen" => desc ? query.OrderByDescending(c => c.TipoExamen) : query.OrderBy(c => c.TipoExamen),
                "resultado" => desc ? query.OrderByDescending(c => c.Resultado) : query.OrderBy(c => c.Resultado),
                "estado" => desc ? query.OrderByDescending(c => c.Estado) : query.OrderBy(c => c.Estado),
                "consultamedicaname" => desc ? query.OrderByDescending(c => c.ConsultaMedica.Paciente.Nombre + " " + c.ConsultaMedica.Paciente.Apellido + " - " + c.ConsultaMedica.Diagnostico) : query.OrderBy(c => c.ConsultaMedica.Paciente.Nombre + " " + c.ConsultaMedica.Paciente.Apellido + " - " + c.ConsultaMedica.Diagnostico),
                "laboratorioname" => desc ? query.OrderByDescending(c => c.Laboratorio.Nombre) : query.OrderBy(c => c.Laboratorio.Nombre),
                _ => desc ? query.OrderByDescending(c => c.TipoExamen) : query.OrderBy(c => c.TipoExamen)
            };

            var data = await query
                .Skip(request.start)
                .Take(request.length > 0 ? request.length : 10)
                .Select(c => new ExamenMedicoTableDTO
                {
                    id = c.Id,
                    tipoExamen = c.TipoExamen,
                    resultado = c.Resultado,
                    estado = c.Estado,
                    consultaMedicaId = c.ConsultaMedicaId,
                    consultaMedicaName = c.ConsultaMedica.Paciente.Nombre + " " + c.ConsultaMedica.Paciente.Apellido + " - " + c.ConsultaMedica.Diagnostico,
                    laboratorioId = c.LaboratorioId,
                    laboratorioName = c.Laboratorio.Nombre
                })
                .ToListAsync();

            return new datatableResponseDTO<ExamenMedicoTableDTO>
            {
                draw = request.draw,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsFiltered,
                data = data
            };
        }

        public async Task<bool> UpdateAsync(ExamenMedico examenMedico)
        {
            context.examenesmedicos.Update(examenMedico);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
