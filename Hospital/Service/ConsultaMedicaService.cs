using Hospital.DTOS;
using Hospital.Models;
using Hospital.Repository;

namespace Hospital.Service
{
    // Logica de negocio y validaciones: Consulta médica
    public class ConsultaMedicaService(IConsultaMedicaRepository repo, IDoctorRepository repoDoctor, ICitaRepository repoCita, IPacienteRepository repoPaciente) : IConsultaMedicaService
    {
        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(SaveConsultaMedicaDTO dTO)
        {
            if (await repoDoctor.GetByIdAsync(dTO.doctorId) == null)
                return (false, "El doctor seleccionado no existe");
            if (await repoCita.GetByIdAsync(dTO.citaId) == null)
                return (false, "La cita seleccionada no existe");
            if (await repoPaciente.GetByIdAsync(dTO.pacienteId) == null)
                return (false, "El paciente seleccionado no existe");
            var cita = await repoCita.GetByIdAsync(dTO.citaId);
            if (cita != null && cita.PacienteId != dTO.pacienteId)
                return (false, "La cita seleccionada no pertenece a ese paciente");
            await repo.CreateAsync(new ConsultaMedica
            {
                Fecha = dTO.fecha,
                Diagnostico = dTO.diagnostico.Trim(),
                Tratamiento = dTO.tratamiento.Trim(),
                DoctorId = dTO.doctorId,
                CitaId = dTO.citaId,
                PacienteId = dTO.pacienteId
            });
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id)
        {
            if (await repo.HasDependentsAsync(id))
                return (false, "No se puede eliminar la consulta médica porque tiene registros asociados");
            var deleted = await repo.DeleteAsync(id);
            return deleted ? (true, null) : (false, "Consulta médica no encontrada");
        }

        // Lista para llenar los combos de otros formularios
        public async Task<IReadOnlyList<ConsultaMedicaTableDTO>> GetAllAsync()
        {
            var lista = await repo.GetAllAsync();
            return lista.Select(c => new ConsultaMedicaTableDTO
            {
                id = c.Id,
                fecha = c.Fecha,
                diagnostico = c.Diagnostico,
                tratamiento = c.Tratamiento,
                doctorId = c.DoctorId,
                doctorName = "Dr. " + c.Doctor.Nombre + " " + c.Doctor.Apellido,
                citaId = c.CitaId,
                citaName = c.Cita.Paciente.Nombre + " " + c.Cita.Paciente.Apellido + " - " + c.Cita.Motivo,
                pacienteId = c.PacienteId,
                pacienteName = c.Paciente.Nombre + " " + c.Paciente.Apellido,
                display = c.Paciente.Nombre + " " + c.Paciente.Apellido + " - " + c.Diagnostico
            }).ToList();
        }

        public async Task<SaveConsultaMedicaDTO?> GetByidAsync(int id)
        {
            var consultaMedica = await repo.GetByIdAsync(id);
            if (consultaMedica == null)
                return null;
            return new SaveConsultaMedicaDTO
            {
                Id = consultaMedica.Id,
                fecha = consultaMedica.Fecha,
                diagnostico = consultaMedica.Diagnostico,
                tratamiento = consultaMedica.Tratamiento,
                doctorId = consultaMedica.DoctorId,
                citaId = consultaMedica.CitaId,
                pacienteId = consultaMedica.PacienteId
            };
        }

        public Task<datatableResponseDTO<ConsultaMedicaTableDTO>> GetPagedAsync(DataTableRequestDTO request)
            => repo.GetPagedAsync(request);

        public async Task<(bool Success, string? ErrorMessage)> UpDateAsync(SaveConsultaMedicaDTO dTO)
        {
            if (!dTO.Id.HasValue)
                return (false, "id invalido");
            var consultaMedica = await repo.GetByIdAsync(dTO.Id.Value);
            if (consultaMedica == null)
                return (false, "Consulta médica no encontrada");
            if (await repoDoctor.GetByIdAsync(dTO.doctorId) == null)
                return (false, "El doctor seleccionado no existe");
            if (await repoCita.GetByIdAsync(dTO.citaId) == null)
                return (false, "La cita seleccionada no existe");
            if (await repoPaciente.GetByIdAsync(dTO.pacienteId) == null)
                return (false, "El paciente seleccionado no existe");
            var cita = await repoCita.GetByIdAsync(dTO.citaId);
            if (cita != null && cita.PacienteId != dTO.pacienteId)
                return (false, "La cita seleccionada no pertenece a ese paciente");
            consultaMedica.Fecha = dTO.fecha;
            consultaMedica.Diagnostico = dTO.diagnostico.Trim();
            consultaMedica.Tratamiento = dTO.tratamiento.Trim();
            consultaMedica.DoctorId = dTO.doctorId;
            consultaMedica.CitaId = dTO.citaId;
            consultaMedica.PacienteId = dTO.pacienteId;
            await repo.UpdateAsync(consultaMedica);
            return (true, null);
        }
    }
}
