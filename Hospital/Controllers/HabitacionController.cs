using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital.DTOS;
using Hospital.Service;

namespace Hospital.Controllers
{
    // Controlador de habitaciones: solo recibe la peticion y delega al servicio
    public class HabitacionController(IHabitacionService _habitacionService, IPacienteService _pacienteService) : Controller
    {
        [Authorize(Policy = "habitacion.ver")]
        public async Task<IActionResult> Index()
        {
            ViewBag.Pacientes = await _pacienteService.GetAllAsync();
            return View();
        }
        [Authorize(Policy = "habitacion.ver")]
        [HttpPost]
        public async Task<IActionResult> GetData([FromForm] DataTableRequestDTO request)
        {
            var result = await _habitacionService.GetPagedAsync(request);
            return Ok(result);
        }
        [Authorize(Policy = "habitacion.ver")]
        [HttpPost]
        public async Task<IActionResult> GetByid(int id)
        {
            var result = await _habitacionService.GetByidAsync(id);
            if (result == null)
                return NotFound(new { success = false, message = "Habitación no encontrada" });
            return Ok(result);
        }
        [Authorize(Policy = "habitacion.crear")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SaveHabitacionDTO dTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Datos invalidos" });
            var (sucess, errorMessage) = await _habitacionService.CreateAsync(dTO);
            return sucess ?
                Ok(new { success = true, message = "Habitación creada correctamente" }) :
                BadRequest(new { success = false, message = errorMessage });
        }
        [Authorize(Policy = "habitacion.actualizar")]
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] SaveHabitacionDTO dTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Datos invalidos" });
            var (sucess, errorMessage) = await _habitacionService.UpDateAsync(dTO);
            return sucess ?
                Ok(new { success = true, message = "Habitación actualizada correctamente" }) :
                BadRequest(new { success = false, message = errorMessage });
        }
        [Authorize(Policy = "habitacion.borrar")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var (sucess, errorMessage) = await _habitacionService.DeleteAsync(id);
            return sucess ?
                Ok(new { success = true, message = "Habitación eliminada correctamente" }) :
                BadRequest(new { success = false, message = errorMessage });
        }
    }
}
