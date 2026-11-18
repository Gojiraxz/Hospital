using Hospital.DTOS;
using Hospital.Models;
using Hospital.Repository;

namespace Hospital.Service
{
    // Logica de negocio y validaciones: Cirugía
    public class CirugiaService(ICirugiaRepository repo, IDoctorRepository repoDoctor, IHabitacionRepository repoHabitacion, IPacienteRepository repoPaciente) : ICirugiaService
    {
        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(SaveCirugiaDTO dTO)
        {
            if (await repoDoctor.GetByIdAsync(dTO.doctorId) == null)
                return (false, "El doctor seleccionado no existe");
            if (await repoHabitacion.GetByIdAsync(dTO.habitacionId) == null)
                return (false, "La habitación seleccionada no existe");
            if (await repoPaciente.GetByIdAsync(dTO.pacienteId) == null)
                return (false, "El paciente seleccionado no existe");
            if (await repo.HabitacionOcupadaAsync(dTO.habitacionId, dTO.fecha))
                return (false, "La habitacion ya esta reservada para esa fecha y hora");
            await repo.CreateAsync(new Cirugia
            {
                Nombre = dTO.nombre.Trim(),
                Fecha = dTO.fecha,
                Estado = dTO.estado.Trim(),
                DoctorId = dTO.doctorId,
                HabitacionId = dTO.habitacionId,
                PacienteId = dTO.pacienteId
            });
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id)
        {
            var deleted = await repo.DeleteAsync(id);
            return deleted ? (true, null) : (false, "Cirugía no encontrada");
        }

        public async Task<SaveCirugiaDTO?> GetByidAsync(int id)
        {
            var cirugia = await repo.GetByIdAsync(id);
            if (cirugia == null)
                return null;
            return new SaveCirugiaDTO
            {
                Id = cirugia.Id,
                nombre = cirugia.Nombre,
                fecha = cirugia.Fecha,
                estado = cirugia.Estado,
                doctorId = cirugia.DoctorId,
                habitacionId = cirugia.HabitacionId,
                pacienteId = cirugia.PacienteId
            };
        }

        public Task<datatableResponseDTO<CirugiaTableDTO>> GetPagedAsync(DataTableRequestDTO request)
            => repo.GetPagedAsync(request);

        public async Task<(bool Success, string? ErrorMessage)> UpDateAsync(SaveCirugiaDTO dTO)
        {
            if (!dTO.Id.HasValue)
                return (false, "id invalido");
            var cirugia = await repo.GetByIdAsync(dTO.Id.Value);
            if (cirugia == null)
                return (false, "Cirugía no encontrada");
            if (await repoDoctor.GetByIdAsync(dTO.doctorId) == null)
                return (false, "El doctor seleccionado no existe");
            if (await repoHabitacion.GetByIdAsync(dTO.habitacionId) == null)
                return (false, "La habitación seleccionada no existe");
            if (await repoPaciente.GetByIdAsync(dTO.pacienteId) == null)
                return (false, "El paciente seleccionado no existe");
            if (await repo.HabitacionOcupadaAsync(dTO.habitacionId, dTO.fecha, dTO.Id))
                return (false, "La habitacion ya esta reservada para esa fecha y hora");
            cirugia.Nombre = dTO.nombre.Trim();
            cirugia.Fecha = dTO.fecha;
            cirugia.Estado = dTO.estado.Trim();
            cirugia.DoctorId = dTO.doctorId;
            cirugia.HabitacionId = dTO.habitacionId;
            cirugia.PacienteId = dTO.pacienteId;
            await repo.UpdateAsync(cirugia);
            return (true, null);
        }
    }
}
