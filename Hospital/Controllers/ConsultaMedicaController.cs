using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital.DTOS;
using Hospital.Service;

namespace Hospital.Controllers
{
    // Controlador de consultas médicas: solo recibe la peticion y delega al servicio
    public class ConsultaMedicaController(IConsultaMedicaService _consultaMedicaService, IDoctorService _doctorService, ICitaService _citaService, IPacienteService _pacienteService) : Controller
    {
        [Authorize(Policy = "consulta.ver")]
        public async Task<IActionResult> Index()
        {
            ViewBag.Doctores = await _doctorService.GetAllAsync();
            ViewBag.Citas = await _citaService.GetAllAsync();
            ViewBag.Pacientes = await _pacienteService.GetAllAsync();
            return View();
        }
        [Authorize(Policy = "consulta.ver")]
        [HttpPost]
        public async Task<IActionResult> GetData([FromForm] DataTableRequestDTO request)
        {
            var result = await _consultaMedicaService.GetPagedAsync(request);
            return Ok(result);
        }
        [Authorize(Policy = "consulta.ver")]
        [HttpPost]
        public async Task<IActionResult> GetByid(int id)
        {
            var result = await _consultaMedicaService.GetByidAsync(id);
            if (result == null)
                return NotFound(new { success = false, message = "Consulta médica no encontrada" });
            return Ok(result);
        }
        [Authorize(Policy = "consulta.crear")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SaveConsultaMedicaDTO dTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Datos invalidos" });
            var (sucess, errorMessage) = await _consultaMedicaService.CreateAsync(dTO);
            return sucess ?
                Ok(new { success = true, message = "Consulta médica creada correctamente" }) :
                BadRequest(new { success = false, message = errorMessage });
        }
        [Authorize(Policy = "consulta.actualizar")]
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] SaveConsultaMedicaDTO dTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Datos invalidos" });
            var (sucess, errorMessage) = await _consultaMedicaService.UpDateAsync(dTO);
            return sucess ?
                Ok(new { success = true, message = "Consulta médica actualizada correctamente" }) :
                BadRequest(new { success = false, message = errorMessage });
        }
        [Authorize(Policy = "consulta.borrar")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var (sucess, errorMessage) = await _consultaMedicaService.DeleteAsync(id);
            return sucess ?
                Ok(new { success = true, message = "Consulta médica eliminada correctamente" }) :
                BadRequest(new { success = false, message = errorMessage });
        }
    }
}
