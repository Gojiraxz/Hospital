using Hospital.DTOS;
using Hospital.Models;
using Hospital.Repository;

namespace Hospital.Service
{
    // Logica de negocio y validaciones: Examen médico
    public class ExamenMedicoService(IExamenMedicoRepository repo, IConsultaMedicaRepository repoConsultaMedica, ILaboratorioRepository repoLaboratorio) : IExamenMedicoService
    {
        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(SaveExamenMedicoDTO dTO)
        {
            if (await repoConsultaMedica.GetByIdAsync(dTO.consultaMedicaId) == null)
                return (false, "La consulta médica seleccionada no existe");
            if (await repoLaboratorio.GetByIdAsync(dTO.laboratorioId) == null)
                return (false, "El laboratorio seleccionado no existe");
            await repo.CreateAsync(new ExamenMedico
            {
                TipoExamen = dTO.tipoExamen.Trim(),
                Resultado = dTO.resultado.Trim(),
                Estado = dTO.estado.Trim(),
                ConsultaMedicaId = dTO.consultaMedicaId,
                LaboratorioId = dTO.laboratorioId
            });
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id)
        {
            var deleted = await repo.DeleteAsync(id);
            return deleted ? (true, null) : (false, "Examen médico no encontrado");
        }

        public async Task<SaveExamenMedicoDTO?> GetByidAsync(int id)
        {
            var examenMedico = await repo.GetByIdAsync(id);
            if (examenMedico == null)
                return null;
            return new SaveExamenMedicoDTO
            {
                Id = examenMedico.Id,
                tipoExamen = examenMedico.TipoExamen,
                resultado = examenMedico.Resultado,
                estado = examenMedico.Estado,
                consultaMedicaId = examenMedico.ConsultaMedicaId,
                laboratorioId = examenMedico.LaboratorioId
            };
        }

        public Task<datatableResponseDTO<ExamenMedicoTableDTO>> GetPagedAsync(DataTableRequestDTO request)
            => repo.GetPagedAsync(request);

        public async Task<(bool Success, string? ErrorMessage)> UpDateAsync(SaveExamenMedicoDTO dTO)
        {
            if (!dTO.Id.HasValue)
                return (false, "id invalido");
            var examenMedico = await repo.GetByIdAsync(dTO.Id.Value);
            if (examenMedico == null)
                return (false, "Examen médico no encontrado");
            if (await repoConsultaMedica.GetByIdAsync(dTO.consultaMedicaId) == null)
                return (false, "La consulta médica seleccionada no existe");
            if (await repoLaboratorio.GetByIdAsync(dTO.laboratorioId) == null)
                return (false, "El laboratorio seleccionado no existe");
            examenMedico.TipoExamen = dTO.tipoExamen.Trim();
            examenMedico.Resultado = dTO.resultado.Trim();
            examenMedico.Estado = dTO.estado.Trim();
            examenMedico.ConsultaMedicaId = dTO.consultaMedicaId;
            examenMedico.LaboratorioId = dTO.laboratorioId;
            await repo.UpdateAsync(examenMedico);
            return (true, null);
        }
    }
}
