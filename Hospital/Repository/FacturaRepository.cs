using Microsoft.EntityFrameworkCore;
using Hospital.Data;
using Hospital.DTOS;
using Hospital.Models;

namespace Hospital.Repository
{
    // Acceso a datos con EF Core: Factura
    public class FacturaRepository(AppDBcontext context) : IFacturaRepository
    {
        public async Task<Factura> CreateAsync(Factura factura)
        {
            var entry = await context.facturas.AddAsync(factura);
            await context.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var factura = await context.facturas.FindAsync(id);
            if (factura == null)
                return false;
            context.facturas.Remove(factura);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<IReadOnlyList<Factura>> GetAllAsync()
        {
            return await context.facturas.Include(x => x.Paciente).OrderBy(x => x.Fecha).ToListAsync();
        }

        public async Task<Factura?> GetByIdAsync(int id)
        {
            return await context.facturas.FindAsync(id);
        }

        public async Task<datatableResponseDTO<FacturaTableDTO>> GetPagedAsync(DataTableRequestDTO request)
        {
            var query = context.facturas.AsQueryable();
            // Se esperan secuencialmente: un mismo DbContext no admite consultas simultaneas
            var recordsTotal = await query.CountAsync();

            var search = request.search?.value?.Trim();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.Estado.Contains(search)
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
                "total" => desc ? query.OrderByDescending(c => c.Total) : query.OrderBy(c => c.Total),
                "estado" => desc ? query.OrderByDescending(c => c.Estado) : query.OrderBy(c => c.Estado),
                "pacientename" => desc ? query.OrderByDescending(c => c.Paciente.Nombre + " " + c.Paciente.Apellido) : query.OrderBy(c => c.Paciente.Nombre + " " + c.Paciente.Apellido),
                _ => desc ? query.OrderByDescending(c => c.Fecha) : query.OrderBy(c => c.Fecha)
            };

            var data = await query
                .Skip(request.start)
                .Take(request.length > 0 ? request.length : 10)
                .Select(c => new FacturaTableDTO
                {
                    id = c.Id,
                    fecha = c.Fecha,
                    total = c.Total,
                    estado = c.Estado,
                    pacienteId = c.PacienteId,
                    pacienteName = c.Paciente.Nombre + " " + c.Paciente.Apellido,
                    display = "Factura de " + c.Paciente.Nombre + " " + c.Paciente.Apellido + " (" + c.Estado + ")"
                })
                .ToListAsync();

            return new datatableResponseDTO<FacturaTableDTO>
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
            return await context.detallesfactura.AnyAsync(x => x.FacturaId == id);
        }

        // Recalcula el total de la factura sumando los subtotales de su detalle
        public async Task RecalcularTotalAsync(int facturaId)
        {
            var total = await context.detallesfactura
                .Where(d => d.FacturaId == facturaId)
                .SumAsync(d => (decimal?)d.Subtotal) ?? 0;
            var factura = await context.facturas.FindAsync(facturaId);
            if (factura == null)
                return;
            factura.Total = total;
            await context.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(Factura factura)
        {
            context.facturas.Update(factura);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
