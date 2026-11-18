namespace Hospital.DTOS
{
    // DTO para mostrar exámenes médicos en el DataTable
    public class ExamenMedicoTableDTO
    {
        public int id { get; set; }
        public string tipoExamen { get; set; } = string.Empty;
        public string resultado { get; set; } = string.Empty;
        public string estado { get; set; } = string.Empty;
        public int consultaMedicaId { get; set; }
        public string consultaMedicaName { get; set; } = string.Empty;
        public int laboratorioId { get; set; }
        public string laboratorioName { get; set; } = string.Empty;
    }
}
