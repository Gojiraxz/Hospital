using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital.DTOS;
using Hospital.Service;

namespace Hospital.Controllers
{
    // Controlador de laboratorios: solo recibe la peticion y delega al servicio
    public class LaboratorioController(ILaboratorioService _laboratorioService) : Controller
    {
        [Authorize(Policy = "laboratorio.ver")]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Policy = "laboratorio.ver")]
        [HttpPost]
        public async Task<IActionResult> GetData([FromForm] DataTableRequestDTO request)
        {
            var result = await _laboratorioService.GetPagedAsync(request);
            return Ok(result);
        }
        [Authorize(Policy = "laboratorio.ver")]
        [HttpPost]
        public async Task<IActionResult> GetByid(int id)
        {
            var result = await _laboratorioService.GetByidAsync(id);
            if (result == null)
                return NotFound(new { success = false, message = "Laboratorio no encontrado" });
            return Ok(result);
        }
        [Authorize(Policy = "laboratorio.crear")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SaveLaboratorioDTO dTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Datos invalidos" });
            var (sucess, errorMessage) = await _laboratorioService.CreateAsync(dTO);
            return sucess ?
                Ok(new { success = true, message = "Laboratorio creado correctamente" }) :
                BadRequest(new { success = false, message = errorMessage });
        }
        [Authorize(Policy = "laboratorio.actualizar")]
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] SaveLaboratorioDTO dTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Datos invalidos" });
            var (sucess, errorMessage) = await _laboratorioService.UpDateAsync(dTO);
            return sucess ?
                Ok(new { success = true, message = "Laboratorio actualizado correctamente" }) :
                BadRequest(new { success = false, message = errorMessage });
        }
        [Authorize(Policy = "laboratorio.borrar")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var (sucess, errorMessage) = await _laboratorioService.DeleteAsync(id);
            return sucess ?
                Ok(new { success = true, message = "Laboratorio eliminado correctamente" }) :
                BadRequest(new { success = false, message = errorMessage });
        }
    }
}
