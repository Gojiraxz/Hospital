using Hospital.DTOS;
using Hospital.Models;
using Hospital.Repository;

namespace Hospital.Service
{
    // Logica de negocio y validaciones: Cita
    public class CitaService(ICitaRepository repo, IDoctorRepository repoDoctor, IPacienteRepository repoPaciente) : ICitaService
    {
        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(SaveCitaDTO dTO)
        {
            if (await repoDoctor.GetByIdAsync(dTO.doctorId) == null)
                return (false, "El doctor seleccionado no existe");
            if (await repoPaciente.GetByIdAsync(dTO.pacienteId) == null)
                return (false, "El paciente seleccionado no existe");
            if (dTO.fecha < DateTime.Now)
                return (false, "La fecha de la cita no puede estar en el pasado");
            if (await repo.DoctorOcupadoAsync(dTO.doctorId, dTO.fecha))
                return (false, "El doctor ya tiene una cita a esa hora");
            await repo.CreateAsync(new Cita
            {
                Fecha = dTO.fecha,
                Estado = dTO.estado.Trim(),
                Motivo = dTO.motivo.Trim(),
                DoctorId = dTO.doctorId,
                PacienteId = dTO.pacienteId
            });
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id)
        {
            if (await repo.HasDependentsAsync(id))
                return (false, "No se puede eliminar la cita porque tiene registros asociados");
            var deleted = await repo.DeleteAsync(id);
            return deleted ? (true, null) : (false, "Cita no encontrada");
        }

        // Lista para llenar los combos de otros formularios
        public async Task<IReadOnlyList<CitaTableDTO>> GetAllAsync()
        {
            var lista = await repo.GetAllAsync();
            return lista.Select(c => new CitaTableDTO
            {
                id = c.Id,
                fecha = c.Fecha,
                estado = c.Estado,
                motivo = c.Motivo,
                doctorId = c.DoctorId,
                doctorName = "Dr. " + c.Doctor.Nombre + " " + c.Doctor.Apellido,
                pacienteId = c.PacienteId,
                pacienteName = c.Paciente.Nombre + " " + c.Paciente.Apellido,
                display = c.Paciente.Nombre + " " + c.Paciente.Apellido + " - " + c.Motivo
            }).ToList();
        }

        public async Task<IReadOnlyList<CitaTableDTO>> GetByPacienteAsync(int pacienteId)
        {
            var lista = await repo.GetByPacienteAsync(pacienteId);
            return lista.Select(c => new CitaTableDTO
            {
                id = c.Id,
                fecha = c.Fecha,
                estado = c.Estado,
                motivo = c.Motivo,
                doctorId = c.DoctorId,
                doctorName = "Dr. " + c.Doctor.Nombre + " " + c.Doctor.Apellido,
                pacienteId = c.PacienteId
            }).ToList();
        }

        public async Task<SaveCitaDTO?> GetByidAsync(int id)
        {
            var cita = await repo.GetByIdAsync(id);
            if (cita == null)
                return null;
            return new SaveCitaDTO
            {
                Id = cita.Id,
                fecha = cita.Fecha,
                estado = cita.Estado,
                motivo = cita.Motivo,
                doctorId = cita.DoctorId,
                pacienteId = cita.PacienteId
            };
        }

        public Task<datatableResponseDTO<CitaTableDTO>> GetPagedAsync(DataTableRequestDTO request)
            => repo.GetPagedAsync(request);

        public async Task<(bool Success, string? ErrorMessage)> UpDateAsync(SaveCitaDTO dTO)
        {
            if (!dTO.Id.HasValue)
                return (false, "id invalido");
            var cita = await repo.GetByIdAsync(dTO.Id.Value);
            if (cita == null)
                return (false, "Cita no encontrada");
            if (await repoDoctor.GetByIdAsync(dTO.doctorId) == null)
                return (false, "El doctor seleccionado no existe");
            if (await repoPaciente.GetByIdAsync(dTO.pacienteId) == null)
                return (false, "El paciente seleccionado no existe");
            if (await repo.DoctorOcupadoAsync(dTO.doctorId, dTO.fecha, dTO.Id))
                return (false, "El doctor ya tiene una cita a esa hora");
            cita.Fecha = dTO.fecha;
            cita.Estado = dTO.estado.Trim();
            cita.Motivo = dTO.motivo.Trim();
            cita.DoctorId = dTO.doctorId;
            cita.PacienteId = dTO.pacienteId;
            await repo.UpdateAsync(cita);
            return (true, null);
        }
    }
}
