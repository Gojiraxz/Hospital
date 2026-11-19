using Hospital.DTOS;
using Hospital.Models;
using Hospital.Repository;

namespace Hospital.Service
{
    // Logica de negocio y validaciones: Factura
    public class FacturaService(IFacturaRepository repo, IPacienteRepository repoPaciente) : IFacturaService
    {
        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(SaveFacturaDTO dTO)
        {
            if (await repoPaciente.GetByIdAsync(dTO.pacienteId) == null)
                return (false, "El paciente seleccionado no existe");
            await repo.CreateAsync(new Factura
            {
                Fecha = dTO.fecha,
                Total = 0,
                Estado = dTO.estado.Trim(),
                PacienteId = dTO.pacienteId
            });
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id)
        {
            if (await repo.HasDependentsAsync(id))
                return (false, "No se puede eliminar la factura porque tiene registros asociados");
            var deleted = await repo.DeleteAsync(id);
            return deleted ? (true, null) : (false, "Factura no encontrada");
        }

        // Lista para llenar los combos de otros formularios
        public async Task<IReadOnlyList<FacturaTableDTO>> GetAllAsync()
        {
            var lista = await repo.GetAllAsync();
            return lista.Select(c => new FacturaTableDTO
            {
                id = c.Id,
                fecha = c.Fecha,
                total = c.Total,
                estado = c.Estado,
                pacienteId = c.PacienteId,
                pacienteName = c.Paciente.Nombre + " " + c.Paciente.Apellido,
                display = "Factura de " + c.Paciente.Nombre + " " + c.Paciente.Apellido + " (" + c.Estado + ")"
            }).ToList();
        }

        public async Task<SaveFacturaDTO?> GetByidAsync(int id)
        {
            var factura = await repo.GetByIdAsync(id);
            if (factura == null)
                return null;
            return new SaveFacturaDTO
            {
                Id = factura.Id,
                fecha = factura.Fecha,
                estado = factura.Estado,
                pacienteId = factura.PacienteId
            };
        }

        public Task<datatableResponseDTO<FacturaTableDTO>> GetPagedAsync(DataTableRequestDTO request)
            => repo.GetPagedAsync(request);

        public async Task<(bool Success, string? ErrorMessage)> UpDateAsync(SaveFacturaDTO dTO)
        {
            if (!dTO.Id.HasValue)
                return (false, "id invalido");
            var factura = await repo.GetByIdAsync(dTO.Id.Value);
            if (factura == null)
                return (false, "Factura no encontrada");
            if (await repoPaciente.GetByIdAsync(dTO.pacienteId) == null)
                return (false, "El paciente seleccionado no existe");
            factura.Fecha = dTO.fecha;
            factura.Estado = dTO.estado.Trim();
            factura.PacienteId = dTO.pacienteId;
            await repo.UpdateAsync(factura);
            return (true, null);
        }
    }
}
