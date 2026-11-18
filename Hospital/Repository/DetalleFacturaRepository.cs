using Microsoft.EntityFrameworkCore;
using Hospital.Data;
using Hospital.DTOS;
using Hospital.Models;

namespace Hospital.Repository
{
    // Acceso a datos con EF Core: Detalle de factura
    public class DetalleFacturaRepository(AppDBcontext context) : IDetalleFacturaRepository
    {
        public async Task<DetalleFactura> CreateAsync(DetalleFactura detalleFactura)
        {
            var entry = await context.detallesfactura.AddAsync(detalleFactura);
            await context.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var detalleFactura = await context.detallesfactura.FindAsync(id);
            if (detalleFactura == null)
                return false;
            context.detallesfactura.Remove(detalleFactura);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<DetalleFactura?> GetByIdAsync(int id)
        {
            return await context.detallesfactura.FindAsync(id);
        }

        public async Task<datatableResponseDTO<DetalleFacturaTableDTO>> GetPagedAsync(DataTableRequestDTO request)
        {
            var query = context.detallesfactura.AsQueryable();
            // Se esperan secuencialmente: un mismo DbContext no admite consultas simultaneas
            var recordsTotal = await query.CountAsync();

            var search = request.search?.value?.Trim();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => ("Factura de " + c.Factura.Paciente.Nombre + " " + c.Factura.Paciente.Apellido + " (" + c.Factura.Estado + ")").Contains(search)
                    || (c.Medicamento.Nombre).Contains(search));
            }
            var recordsFiltered = await query.CountAsync();

            var order = request.order.FirstOrDefault();
            var desc = string.Equals(order?.dir, "desc", StringComparison.OrdinalIgnoreCase);
            var columnName = request.columns.ElementAtOrDefault(order?.column ?? 0)?.data ?? "id";

            query = columnName.ToLowerInvariant() switch
            {
                "id" => desc ? query.OrderByDescending(c => c.Id) : query.OrderBy(c => c.Id),
                "cantidad" => desc ? query.OrderByDescending(c => c.Cantidad) : query.OrderBy(c => c.Cantidad),
                "preciounitario" => desc ? query.OrderByDescending(c => c.PrecioUnitario) : query.OrderBy(c => c.PrecioUnitario),
                "subtotal" => desc ? query.OrderByDescending(c => c.Subtotal) : query.OrderBy(c => c.Subtotal),
                "facturaname" => desc ? query.OrderByDescending(c => "Factura de " + c.Factura.Paciente.Nombre + " " + c.Factura.Paciente.Apellido + " (" + c.Factura.Estado + ")") : query.OrderBy(c => "Factura de " + c.Factura.Paciente.Nombre + " " + c.Factura.Paciente.Apellido + " (" + c.Factura.Estado + ")"),
                "medicamentoname" => desc ? query.OrderByDescending(c => c.Medicamento.Nombre) : query.OrderBy(c => c.Medicamento.Nombre),
                _ => desc ? query.OrderByDescending(c => c.Id) : query.OrderBy(c => c.Id)
            };

            var data = await query
                .Skip(request.start)
                .Take(request.length > 0 ? request.length : 10)
                .Select(c => new DetalleFacturaTableDTO
                {
                    id = c.Id,
                    cantidad = c.Cantidad,
                    precioUnitario = c.PrecioUnitario,
                    subtotal = c.Subtotal,
                    facturaId = c.FacturaId,
                    facturaName = "Factura de " + c.Factura.Paciente.Nombre + " " + c.Factura.Paciente.Apellido + " (" + c.Factura.Estado + ")",
                    medicamentoId = c.MedicamentoId,
                    medicamentoName = c.Medicamento.Nombre
                })
                .ToListAsync();

            return new datatableResponseDTO<DetalleFacturaTableDTO>
            {
                draw = request.draw,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsFiltered,
                data = data
            };
        }

        public async Task<bool> UpdateAsync(DetalleFactura detalleFactura)
        {
            context.detallesfactura.Update(detalleFactura);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
