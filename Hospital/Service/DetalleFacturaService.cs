using Hospital.DTOS;
using Hospital.Models;
using Hospital.Repository;

namespace Hospital.Service
{
    // Logica de negocio y validaciones: Detalle de factura
    public class DetalleFacturaService(IDetalleFacturaRepository repo, IFacturaRepository repoFactura, IMedicamentoRepository repoMedicamento) : IDetalleFacturaService
    {
        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(SaveDetalleFacturaDTO dTO)
        {
            if (await repoFactura.GetByIdAsync(dTO.facturaId) == null)
                return (false, "La factura seleccionada no existe");
            if (await repoMedicamento.GetByIdAsync(dTO.medicamentoId) == null)
                return (false, "El medicamento seleccionado no existe");
            await repo.CreateAsync(new DetalleFactura
            {
                Cantidad = dTO.cantidad,
                PrecioUnitario = dTO.precioUnitario,
                Subtotal = dTO.cantidad * dTO.precioUnitario,
                FacturaId = dTO.facturaId,
                MedicamentoId = dTO.medicamentoId
            });
            await repoFactura.RecalcularTotalAsync(dTO.facturaId);
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id)
        {
            var detalle = await repo.GetByIdAsync(id);
            var deleted = await repo.DeleteAsync(id);
            if (deleted && detalle != null)
                await repoFactura.RecalcularTotalAsync(detalle.FacturaId);
            return deleted ? (true, null) : (false, "Detalle de factura no encontrado");
        }

        public async Task<SaveDetalleFacturaDTO?> GetByidAsync(int id)
        {
            var detalleFactura = await repo.GetByIdAsync(id);
            if (detalleFactura == null)
                return null;
            return new SaveDetalleFacturaDTO
            {
                Id = detalleFactura.Id,
                cantidad = detalleFactura.Cantidad,
                precioUnitario = detalleFactura.PrecioUnitario,
                facturaId = detalleFactura.FacturaId,
                medicamentoId = detalleFactura.MedicamentoId
            };
        }

        public Task<datatableResponseDTO<DetalleFacturaTableDTO>> GetPagedAsync(DataTableRequestDTO request)
            => repo.GetPagedAsync(request);

        public async Task<(bool Success, string? ErrorMessage)> UpDateAsync(SaveDetalleFacturaDTO dTO)
        {
            if (!dTO.Id.HasValue)
                return (false, "id invalido");
            var detalleFactura = await repo.GetByIdAsync(dTO.Id.Value);
            if (detalleFactura == null)
                return (false, "Detalle de factura no encontrado");
            if (await repoFactura.GetByIdAsync(dTO.facturaId) == null)
                return (false, "La factura seleccionada no existe");
            if (await repoMedicamento.GetByIdAsync(dTO.medicamentoId) == null)
                return (false, "El medicamento seleccionado no existe");
            var facturaAnterior = detalleFactura.FacturaId;
            detalleFactura.Cantidad = dTO.cantidad;
            detalleFactura.PrecioUnitario = dTO.precioUnitario;
            detalleFactura.Subtotal = dTO.cantidad * dTO.precioUnitario;
            detalleFactura.FacturaId = dTO.facturaId;
            detalleFactura.MedicamentoId = dTO.medicamentoId;
            await repo.UpdateAsync(detalleFactura);
            await repoFactura.RecalcularTotalAsync(detalleFactura.FacturaId);
            if (facturaAnterior != detalleFactura.FacturaId)
                await repoFactura.RecalcularTotalAsync(facturaAnterior);
            return (true, null);
        }
    }
}
