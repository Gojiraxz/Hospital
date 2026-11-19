using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital.DTOS;
using Hospital.Service;

namespace Hospital.Controllers
{
    // Controlador de enfermeros: solo recibe la peticion y delega al servicio
    public class EnfermeroController(IEnfermeroService _enfermeroService, IEspecialidadService _especialidadService) : Controller
    {
        [Authorize(Policy = "enfermero.ver")]
        public async Task<IActionResult> Index()
        {
            ViewBag.Especialidades = await _especialidadService.GetAllAsync();
            return View();
        }
        [Authorize(Policy = "enfermero.ver")]
        [HttpPost]
        public async Task<IActionResult> GetData([FromForm] DataTableRequestDTO request)
        {
            var result = await _enfermeroService.GetPagedAsync(request);
            return Ok(result);
        }
        [Authorize(Policy = "enfermero.ver")]
        [HttpPost]
        public async Task<IActionResult> GetByid(int id)
        {
            var result = await _enfermeroService.GetByidAsync(id);
            if (result == null)
                return NotFound(new { success = false, message = "Enfermero no encontrado" });
            return Ok(result);
        }
        [Authorize(Policy = "enfermero.crear")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SaveEnfermeroDTO dTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Datos invalidos" });
            var (sucess, errorMessage) = await _enfermeroService.CreateAsync(dTO);
            return sucess ?
                Ok(new { success = true, message = "Enfermero creado correctamente" }) :
                BadRequest(new { success = false, message = errorMessage });
        }
        [Authorize(Policy = "enfermero.actualizar")]
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] SaveEnfermeroDTO dTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Datos invalidos" });
            var (sucess, errorMessage) = await _enfermeroService.UpDateAsync(dTO);
            return sucess ?
                Ok(new { success = true, message = "Enfermero actualizado correctamente" }) :
                BadRequest(new { success = false, message = errorMessage });
        }
        [Authorize(Policy = "enfermero.borrar")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var (sucess, errorMessage) = await _enfermeroService.DeleteAsync(id);
            return sucess ?
                Ok(new { success = true, message = "Enfermero eliminado correctamente" }) :
                BadRequest(new { success = false, message = errorMessage });
        }
    }
}
