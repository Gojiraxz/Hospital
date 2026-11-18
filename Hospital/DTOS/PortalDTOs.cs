using System.ComponentModel.DataAnnotations;

namespace Hospital.DTOS
{
    // El paciente solicita una cita con un doctor
    public class SolicitarCitaDTO
    {
        [Required] public int doctorId { get; set; }
        [Required] public DateTime fecha { get; set; }
        [Required, MaxLength(250)] public string motivo { get; set; } = string.Empty;
    }

    // El paciente solicita una receta a un doctor
    public class SolicitarRecetaDTO
    {
        [Required] public int doctorId { get; set; }
        [Required, MaxLength(150)] public string medicamento { get; set; } = string.Empty;
        [Required, MaxLength(300)] public string indicaciones { get; set; } = string.Empty;
    }

    // El admin/doctor responde una solicitud de receta
    public class ResponderRecetaDTO
    {
        [Required] public int id { get; set; }
        [Required, MaxLength(45)] public string estado { get; set; } = string.Empty;
        [MaxLength(300)] public string? respuesta { get; set; }
    }

    public class RecetaTableDTO
    {
        public int id { get; set; }
        public DateTime fechaSolicitud { get; set; }
        public string medicamento { get; set; } = string.Empty;
        public string indicaciones { get; set; } = string.Empty;
        public string estado { get; set; } = string.Empty;
        public string? respuesta { get; set; }
        public int doctorId { get; set; }
        public string doctorName { get; set; } = string.Empty;
        public int pacienteId { get; set; }
        public string pacienteName { get; set; } = string.Empty;
        public string pacienteCedula { get; set; } = string.Empty;
    }
}
