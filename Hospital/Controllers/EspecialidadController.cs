using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital.DTOS;
using Hospital.Service;

namespace Hospital.Controllers
{
    // Controlador de especialidades: solo recibe la peticion y delega al servicio
    public class EspecialidadController(IEspecialidadService _especialidadService) : Controller
    {
        [Authorize(Policy = "especialidad.ver")]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Policy = "especialidad.ver")]
        [HttpPost]
        public async Task<IActionResult> GetData([FromForm] DataTableRequestDTO request)
        {
            var result = await _especialidadService.GetPagedAsync(request);
            return Ok(result);
        }
        [Authorize(Policy = "especialidad.ver")]
        [HttpPost]
        public async Task<IActionResult> GetByid(int id)
        {
            var result = await _especialidadService.GetByidAsync(id);
            if (result == null)
                return NotFound(new { success = false, message = "Especialidad no encontrada" });
            return Ok(result);
        }
        [Authorize(Policy = "especialidad.crear")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SaveEspecialidadDTO dTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Datos invalidos" });
            var (sucess, errorMessage) = await _especialidadService.CreateAsync(dTO);
            return sucess ?
                Ok(new { success = true, message = "Especialidad creada correctamente" }) :
                BadRequest(new { success = false, message = errorMessage });
        }
        [Authorize(Policy = "especialidad.actualizar")]
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] SaveEspecialidadDTO dTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Datos invalidos" });
            var (sucess, errorMessage) = await _especialidadService.UpDateAsync(dTO);
            return sucess ?
                Ok(new { success = true, message = "Especialidad actualizada correctamente" }) :
                BadRequest(new { success = false, message = errorMessage });
        }
        [Authorize(Policy = "especialidad.borrar")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var (sucess, errorMessage) = await _especialidadService.DeleteAsync(id);
            return sucess ?
                Ok(new { success = true, message = "Especialidad eliminada correctamente" }) :
                BadRequest(new { success = false, message = errorMessage });
        }
    }
}
