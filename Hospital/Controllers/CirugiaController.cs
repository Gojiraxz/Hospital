using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital.DTOS;
using Hospital.Service;

namespace Hospital.Controllers
{
    // Controlador de cirugías: solo recibe la peticion y delega al servicio
    public class CirugiaController(ICirugiaService _cirugiaService, IDoctorService _doctorService, IHabitacionService _habitacionService, IPacienteService _pacienteService) : Controller
    {
        [Authorize(Policy = "cirugia.ver")]
        public async Task<IActionResult> Index()
        {
            ViewBag.Doctores = await _doctorService.GetAllAsync();
            ViewBag.Habitaciones = await _habitacionService.GetAllAsync();
            ViewBag.Pacientes = await _pacienteService.GetAllAsync();
            return View();
        }
        [Authorize(Policy = "cirugia.ver")]
        [HttpPost]
        public async Task<IActionResult> GetData([FromForm] DataTableRequestDTO request)
        {
            var result = await _cirugiaService.GetPagedAsync(request);
            return Ok(result);
        }
        [Authorize(Policy = "cirugia.ver")]
        [HttpPost]
        public async Task<IActionResult> GetByid(int id)
        {
            var result = await _cirugiaService.GetByidAsync(id);
            if (result == null)
                return NotFound(new { success = false, message = "Cirugía no encontrada" });
            return Ok(result);
        }
        [Authorize(Policy = "cirugia.crear")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SaveCirugiaDTO dTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Datos invalidos" });
            var (sucess, errorMessage) = await _cirugiaService.CreateAsync(dTO);
            return sucess ?
                Ok(new { success = true, message = "Cirugía creada correctamente" }) :
                BadRequest(new { success = false, message = errorMessage });
        }
        [Authorize(Policy = "cirugia.actualizar")]
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] SaveCirugiaDTO dTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Datos invalidos" });
            var (sucess, errorMessage) = await _cirugiaService.UpDateAsync(dTO);
            return sucess ?
                Ok(new { success = true, message = "Cirugía actualizada correctamente" }) :
                BadRequest(new { success = false, message = errorMessage });
        }
        [Authorize(Policy = "cirugia.borrar")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var (sucess, errorMessage) = await _cirugiaService.DeleteAsync(id);
            return sucess ?
                Ok(new { success = true, message = "Cirugía eliminada correctamente" }) :
                BadRequest(new { success = false, message = errorMessage });
        }
    }
}
