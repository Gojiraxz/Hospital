using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital.DTOS;
using Hospital.Service;

namespace Hospital.Controllers
{
    // Panel del admin/doctor: ve y responde las solicitudes de recetas de los pacientes
    public class RecetaController(IRecetaService _recetaService) : Controller
    {
        [Authorize(Policy = "receta.ver")]
        public IActionResult Index() => View();

        [Authorize(Policy = "receta.ver")]
        [HttpPost]
        public async Task<IActionResult> GetData([FromForm] DataTableRequestDTO request)
        {
            var result = await _recetaService.GetPagedAsync(request);
            return Ok(result);
        }

        [Authorize(Policy = "receta.actualizar")]
        [HttpPost]
        public async Task<IActionResult> Responder([FromBody] ResponderRecetaDTO dTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Datos invalidos" });
            var (success, errorMessage) = await _recetaService.ResponderAsync(dTO);
            return success
                ? Ok(new { success = true, message = "Solicitud de receta actualizada" })
                : BadRequest(new { success = false, message = errorMessage });
        }
    }
}
