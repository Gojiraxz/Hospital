using System.ComponentModel.DataAnnotations;

namespace Hospital.DTOS
{
    // Inicio de sesion del paciente: solo cedula (registrada previamente por el admin)
    public class PacienteLoginDTO
    {
        [Required]
        public string cedula { get; set; } = string.Empty;
    }
}
