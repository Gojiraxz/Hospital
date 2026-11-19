using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital.DTOS;
using Hospital.Service;

namespace Hospital.Controllers
{
    // Controlador de exámenes médicos: solo recibe la peticion y delega al servicio
    public class ExamenMedicoController(IExamenMedicoService _examenMedicoService, IConsultaMedicaService _consultaMedicaService, ILaboratorioService _laboratorioService) : Controller
    {
        [Authorize(Policy = "examen.ver")]
        public async Task<IActionResult> Index()
        {
            ViewBag.ConsultasMedicas = await _consultaMedicaService.GetAllAsync();
            ViewBag.Laboratorios = await _laboratorioService.GetAllAsync();
            return View();
        }
        [Authorize(Policy = "examen.ver")]
        [HttpPost]
        public async Task<IActionResult> GetData([FromForm] DataTableRequestDTO request)
        {
            var result = await _examenMedicoService.GetPagedAsync(request);
            return Ok(result);
        }
        [Authorize(Policy = "examen.ver")]
        [HttpPost]
        public async Task<IActionResult> GetByid(int id)
        {
            var result = await _examenMedicoService.GetByidAsync(id);
            if (result == null)
                return NotFound(new { success = false, message = "Examen médico no encontrado" });
            return Ok(result);
        }
        [Authorize(Policy = "examen.crear")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SaveExamenMedicoDTO dTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Datos invalidos" });
            var (sucess, errorMessage) = await _examenMedicoService.CreateAsync(dTO);
            return sucess ?
                Ok(new { success = true, message = "Examen médico creado correctamente" }) :
                BadRequest(new { success = false, message = errorMessage });
        }
        [Authorize(Policy = "examen.actualizar")]
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] SaveExamenMedicoDTO dTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Datos invalidos" });
            var (sucess, errorMessage) = await _examenMedicoService.UpDateAsync(dTO);
            return sucess ?
                Ok(new { success = true, message = "Examen médico actualizado correctamente" }) :
                BadRequest(new { success = false, message = errorMessage });
        }
        [Authorize(Policy = "examen.borrar")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var (sucess, errorMessage) = await _examenMedicoService.DeleteAsync(id);
            return sucess ?
                Ok(new { success = true, message = "Examen médico eliminado correctamente" }) :
                BadRequest(new { success = false, message = errorMessage });
        }
    }
}
