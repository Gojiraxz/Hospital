using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital.DTOS;
using Hospital.Service;

namespace Hospital.Controllers
{
    // Controlador de citas: solo recibe la peticion y delega al servicio
    public class CitaController(ICitaService _citaService, IDoctorService _doctorService, IPacienteService _pacienteService) : Controller
    {
        [Authorize(Policy = "cita.ver")]
        public async Task<IActionResult> Index()
        {
            ViewBag.Doctores = await _doctorService.GetAllAsync();
            ViewBag.Pacientes = await _pacienteService.GetAllAsync();
            return View();
        }
        [Authorize(Policy = "cita.ver")]
        [HttpPost]
        public async Task<IActionResult> GetData([FromForm] DataTableRequestDTO request)
        {
            var result = await _citaService.GetPagedAsync(request);
            return Ok(result);
        }
        [Authorize(Policy = "cita.ver")]
        [HttpPost]
        public async Task<IActionResult> GetByid(int id)
        {
            var result = await _citaService.GetByidAsync(id);
            if (result == null)
                return NotFound(new { success = false, message = "Cita no encontrada" });
            return Ok(result);
        }
        [Authorize(Policy = "cita.crear")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SaveCitaDTO dTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Datos invalidos" });
            var (sucess, errorMessage) = await _citaService.CreateAsync(dTO);
            return sucess ?
                Ok(new { success = true, message = "Cita creada correctamente" }) :
                BadRequest(new { success = false, message = errorMessage });
        }
        [Authorize(Policy = "cita.actualizar")]
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] SaveCitaDTO dTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Datos invalidos" });
            var (sucess, errorMessage) = await _citaService.UpDateAsync(dTO);
            return sucess ?
                Ok(new { success = true, message = "Cita actualizada correctamente" }) :
                BadRequest(new { success = false, message = errorMessage });
        }
        [Authorize(Policy = "cita.borrar")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var (sucess, errorMessage) = await _citaService.DeleteAsync(id);
            return sucess ?
                Ok(new { success = true, message = "Cita eliminada correctamente" }) :
                BadRequest(new { success = false, message = errorMessage });
        }
    }
}
