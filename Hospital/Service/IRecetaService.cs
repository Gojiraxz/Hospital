using Hospital.DTOS;

namespace Hospital.Service
{
    // Contrato de la logica de negocio: Receta
    public interface IRecetaService
    {
        Task<datatableResponseDTO<RecetaTableDTO>> GetPagedAsync(DataTableRequestDTO request);
        Task<IReadOnlyList<RecetaTableDTO>> GetByPacienteAsync(int pacienteId);
        Task<(bool Success, string? ErrorMessage)> SolicitarAsync(int pacienteId, SolicitarRecetaDTO dTO);
        Task<(bool Success, string? ErrorMessage)> ResponderAsync(ResponderRecetaDTO dTO);
    }
}
