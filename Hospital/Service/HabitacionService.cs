using Hospital.DTOS;
using Hospital.Models;
using Hospital.Repository;

namespace Hospital.Service
{
    // Logica de negocio y validaciones: Habitación
    public class HabitacionService(IHabitacionRepository repo, IPacienteRepository repoPaciente) : IHabitacionService
    {
        // Regla: si se asigna un paciente la habitacion queda "Ocupada";
        // si queda Disponible o en Mantenimiento no puede tener paciente.
        private async Task<(bool Success, string? ErrorMessage, string Estado, int? PacienteId)> AplicarReglaOcupacionAsync(SaveHabitacionDTO dTO, int? excludeId)
        {
            var estado = dTO.estado.Trim();
            if (dTO.pacienteId.HasValue)
            {
                if (await repoPaciente.GetByIdAsync(dTO.pacienteId.Value) == null)
                    return (false, "El paciente seleccionado no existe", estado, null);
                if (await repo.PacienteEnOtraHabitacionAsync(dTO.pacienteId.Value, excludeId))
                    return (false, "Ese paciente ya está en otra habitación", estado, null);
                return (true, null, "Ocupada", dTO.pacienteId);
            }
            if (string.Equals(estado, "Ocupada", StringComparison.OrdinalIgnoreCase))
                return (false, "Para marcar la habitación como ocupada selecciona un paciente", estado, null);
            return (true, null, estado, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(SaveHabitacionDTO dTO)
        {
            if (await repo.NumeroExisteAsync(dTO.numero.Trim()))
                return (false, "Ya existe una habitación con ese número");
            var regla = await AplicarReglaOcupacionAsync(dTO, null);
            if (!regla.Success)
                return (false, regla.ErrorMessage);
            await repo.CreateAsync(new Habitacion
            {
                Numero = dTO.numero.Trim(),
                Estado = regla.Estado,
                PacienteId = regla.PacienteId
            });
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id)
        {
            if (await repo.HasDependentsAsync(id))
                return (false, "No se puede eliminar la habitación porque tiene registros asociados");
            var deleted = await repo.DeleteAsync(id);
            return deleted ? (true, null) : (false, "Habitación no encontrada");
        }

        // Lista para llenar los combos de otros formularios
        public async Task<IReadOnlyList<HabitacionTableDTO>> GetAllAsync()
        {
            var lista = await repo.GetAllAsync();
            return lista.Select(c => new HabitacionTableDTO
            {
                id = c.Id,
                numero = c.Numero,
                estado = c.Estado,
                pacienteId = c.PacienteId,
                pacienteName = c.Paciente != null ? c.Paciente.Nombre + " " + c.Paciente.Apellido : "",
                display = "Hab. " + c.Numero
            }).ToList();
        }

        public async Task<SaveHabitacionDTO?> GetByidAsync(int id)
        {
            var habitacion = await repo.GetByIdAsync(id);
            if (habitacion == null)
                return null;
            return new SaveHabitacionDTO
            {
                Id = habitacion.Id,
                numero = habitacion.Numero,
                estado = habitacion.Estado,
                pacienteId = habitacion.PacienteId
            };
        }

        public Task<datatableResponseDTO<HabitacionTableDTO>> GetPagedAsync(DataTableRequestDTO request)
            => repo.GetPagedAsync(request);

        public async Task<(bool Success, string? ErrorMessage)> UpDateAsync(SaveHabitacionDTO dTO)
        {
            if (!dTO.Id.HasValue)
                return (false, "id invalido");
            var habitacion = await repo.GetByIdAsync(dTO.Id.Value);
            if (habitacion == null)
                return (false, "Habitación no encontrada");
            if (await repo.NumeroExisteAsync(dTO.numero.Trim(), dTO.Id))
                return (false, "Ya existe una habitación con ese número");
            var regla = await AplicarReglaOcupacionAsync(dTO, dTO.Id);
            if (!regla.Success)
                return (false, regla.ErrorMessage);
            habitacion.Numero = dTO.numero.Trim();
            habitacion.Estado = regla.Estado;
            habitacion.PacienteId = regla.PacienteId;
            await repo.UpdateAsync(habitacion);
            return (true, null);
        }
    }
}
