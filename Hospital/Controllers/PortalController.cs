using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital.DTOS;
using Hospital.Service;

namespace Hospital.Controllers
{
    // Portal del paciente: ve sus citas/recetas y puede pedir citas o recetas a los doctores
    [Authorize(Roles = "Paciente")]
    public class PortalController(
        IPacienteService _pacienteService,
        IDoctorService _doctorService,
        ICitaService _citaService,
        IRecetaService _recetaService) : Controller
    {
        // Id del paciente que viene en el token (nunca se recibe desde el navegador)
        private int? PacienteId =>
            int.TryParse(User.FindFirst("pacienteId")?.Value, out var id) ? id : null;

        public async Task<IActionResult> Index()
        {
            var pacienteId = PacienteId;
            if (pacienteId == null)
                return RedirectToAction("Login", "Auth");
            var paciente = await _pacienteService.GetByidAsync(pacienteId.Value);
            if (paciente == null)
            {
                // El admin elimino al paciente: se cierra la sesion
                Response.Cookies.Delete("access_token");
                return RedirectToAction("Login", "Auth");
            }
            ViewBag.Paciente = paciente;
            ViewBag.Doctores = await _doctorService.GetAllAsync();
            ViewBag.Citas = await _citaService.GetByPacienteAsync(pacienteId.Value);
            ViewBag.Recetas = await _recetaService.GetByPacienteAsync(pacienteId.Value);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SolicitarCita([FromBody] SolicitarCitaDTO dTO)
        {
            var pacienteId = PacienteId;
            if (pacienteId == null || !ModelState.IsValid)
                return BadRequest(new { success = false, message = "Datos invalidos" });
            var (success, errorMessage) = await _citaService.CreateAsync(new SaveCitaDTO
            {
                fecha = dTO.fecha,
                estado = "Pendiente",
                motivo = dTO.motivo,
                doctorId = dTO.doctorId,
                pacienteId = pacienteId.Value
            });
            return success
                ? Ok(new { success = true, message = "Cita solicitada. Quedó en estado Pendiente." })
                : BadRequest(new { success = false, message = errorMessage });
        }

        [HttpPost]
        public async Task<IActionResult> SolicitarReceta([FromBody] SolicitarRecetaDTO dTO)
        {
            var pacienteId = PacienteId;
            if (pacienteId == null || !ModelState.IsValid)
                return BadRequest(new { success = false, message = "Datos invalidos" });
            var (success, errorMessage) = await _recetaService.SolicitarAsync(pacienteId.Value, dTO);
            return success
                ? Ok(new { success = true, message = "Receta solicitada. El doctor la revisará." })
                : BadRequest(new { success = false, message = errorMessage });
        }
    }
}
