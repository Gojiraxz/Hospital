using Hospital.DTOS;
using Hospital.Models;
using Hospital.Repository;

namespace Hospital.Service
{
    // Logica de negocio y validaciones: Receta
    public class RecetaService(IRecetaRepository repo, IDoctorRepository repoDoctor, IPacienteRepository repoPaciente) : IRecetaService
    {
        private static readonly string[] EstadosValidos = ["Pendiente", "Aprobada", "Rechazada"];

        public Task<datatableResponseDTO<RecetaTableDTO>> GetPagedAsync(DataTableRequestDTO request)
            => repo.GetPagedAsync(request);

        public async Task<IReadOnlyList<RecetaTableDTO>> GetByPacienteAsync(int pacienteId)
        {
            var lista = await repo.GetByPacienteAsync(pacienteId);
            return lista.Select(r => new RecetaTableDTO
            {
                id = r.Id,
                fechaSolicitud = r.FechaSolicitud,
                medicamento = r.Medicamento,
                indicaciones = r.Indicaciones,
                estado = r.Estado,
                respuesta = r.Respuesta,
                doctorId = r.DoctorId,
                doctorName = "Dr. " + r.Doctor.Nombre + " " + r.Doctor.Apellido,
                pacienteId = r.PacienteId
            }).ToList();
        }

        public async Task<(bool Success, string? ErrorMessage)> SolicitarAsync(int pacienteId, SolicitarRecetaDTO dTO)
        {
            if (await repoPaciente.GetByIdAsync(pacienteId) == null)
                return (false, "Paciente no encontrado");
            if (await repoDoctor.GetByIdAsync(dTO.doctorId) == null)
                return (false, "El doctor seleccionado no existe");
            await repo.CreateAsync(new Receta
            {
                FechaSolicitud = DateTime.Now,
                Medicamento = dTO.medicamento.Trim(),
                Indicaciones = dTO.indicaciones.Trim(),
                Estado = "Pendiente",
                DoctorId = dTO.doctorId,
                PacienteId = pacienteId
            });
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> ResponderAsync(ResponderRecetaDTO dTO)
        {
            var estado = EstadosValidos.FirstOrDefault(e => string.Equals(e, dTO.estado?.Trim(), StringComparison.OrdinalIgnoreCase));
            if (estado == null)
                return (false, "Estado invalido");
            var receta = await repo.GetByIdAsync(dTO.id);
            if (receta == null)
                return (false, "Receta no encontrada");
            receta.Estado = estado;
            receta.Respuesta = string.IsNullOrWhiteSpace(dTO.respuesta) ? null : dTO.respuesta.Trim();
            await repo.UpdateAsync(receta);
            return (true, null);
        }
    }
}
