using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital.DTOS;
using Hospital.Service;

namespace Hospital.Controllers
{
    // Controlador de turnos: solo recibe la peticion y delega al servicio
    public class TurnoController(ITurnoService _turnoService, IEnfermeroService _enfermeroService, IDoctorService _doctorService) : Controller
    {
        [Authorize(Policy = "turno.ver")]
        public async Task<IActionResult> Index()
        {
            ViewBag.Enfermeros = await _enfermeroService.GetAllAsync();
            ViewBag.Doctores = await _doctorService.GetAllAsync();
            return View();
        }
        [Authorize(Policy = "turno.ver")]
        [HttpPost]
        public async Task<IActionResult> GetData([FromForm] DataTableRequestDTO request)
        {
            var result = await _turnoService.GetPagedAsync(request);
            return Ok(result);
        }
        [Authorize(Policy = "turno.ver")]
        [HttpPost]
        public async Task<IActionResult> GetByid(int id)
        {
            var result = await _turnoService.GetByidAsync(id);
            if (result == null)
                return NotFound(new { success = false, message = "Turno no encontrado" });
            return Ok(result);
        }
        [Authorize(Policy = "turno.crear")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SaveTurnoDTO dTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Datos invalidos" });
            var (sucess, errorMessage) = await _turnoService.CreateAsync(dTO);
            return sucess ?
                Ok(new { success = true, message = "Turno creado correctamente" }) :
                BadRequest(new { success = false, message = errorMessage });
        }
        [Authorize(Policy = "turno.actualizar")]
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] SaveTurnoDTO dTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Datos invalidos" });
            var (sucess, errorMessage) = await _turnoService.UpDateAsync(dTO);
            return sucess ?
                Ok(new { success = true, message = "Turno actualizado correctamente" }) :
                BadRequest(new { success = false, message = errorMessage });
        }
        [Authorize(Policy = "turno.borrar")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var (sucess, errorMessage) = await _turnoService.DeleteAsync(id);
            return sucess ?
                Ok(new { success = true, message = "Turno eliminado correctamente" }) :
                BadRequest(new { success = false, message = errorMessage });
        }
    }
}
