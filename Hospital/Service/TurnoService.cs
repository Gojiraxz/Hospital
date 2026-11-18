using Hospital.DTOS;
using Hospital.Models;
using Hospital.Repository;

namespace Hospital.Service
{
    // Logica de negocio y validaciones: Turno
    public class TurnoService(ITurnoRepository repo, IEnfermeroRepository repoEnfermero, IDoctorRepository repoDoctor) : ITurnoService
    {
        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(SaveTurnoDTO dTO)
        {
            if (await repoEnfermero.GetByIdAsync(dTO.enfermeroId) == null)
                return (false, "El enfermero seleccionado no existe");
            if (await repoDoctor.GetByIdAsync(dTO.doctorId) == null)
                return (false, "El doctor seleccionado no existe");
            if (string.CompareOrdinal(dTO.final, dTO.inicio) <= 0)
                return (false, "La hora final debe ser mayor a la hora de inicio");
            await repo.CreateAsync(new Turno
            {
                Inicio = dTO.inicio.Trim(),
                Final = dTO.final.Trim(),
                EnfermeroId = dTO.enfermeroId,
                DoctorId = dTO.doctorId
            });
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id)
        {
            var deleted = await repo.DeleteAsync(id);
            return deleted ? (true, null) : (false, "Turno no encontrado");
        }

        public async Task<SaveTurnoDTO?> GetByidAsync(int id)
        {
            var turno = await repo.GetByIdAsync(id);
            if (turno == null)
                return null;
            return new SaveTurnoDTO
            {
                Id = turno.Id,
                inicio = turno.Inicio,
                final = turno.Final,
                enfermeroId = turno.EnfermeroId,
                doctorId = turno.DoctorId
            };
        }

        public Task<datatableResponseDTO<TurnoTableDTO>> GetPagedAsync(DataTableRequestDTO request)
            => repo.GetPagedAsync(request);

        public async Task<(bool Success, string? ErrorMessage)> UpDateAsync(SaveTurnoDTO dTO)
        {
            if (!dTO.Id.HasValue)
                return (false, "id invalido");
            var turno = await repo.GetByIdAsync(dTO.Id.Value);
            if (turno == null)
                return (false, "Turno no encontrado");
            if (await repoEnfermero.GetByIdAsync(dTO.enfermeroId) == null)
                return (false, "El enfermero seleccionado no existe");
            if (await repoDoctor.GetByIdAsync(dTO.doctorId) == null)
                return (false, "El doctor seleccionado no existe");
            if (string.CompareOrdinal(dTO.final, dTO.inicio) <= 0)
                return (false, "La hora final debe ser mayor a la hora de inicio");
            turno.Inicio = dTO.inicio.Trim();
            turno.Final = dTO.final.Trim();
            turno.EnfermeroId = dTO.enfermeroId;
            turno.DoctorId = dTO.doctorId;
            await repo.UpdateAsync(turno);
            return (true, null);
        }
    }
}
