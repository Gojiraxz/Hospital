using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital.DTOS;
using Hospital.Service;

namespace Hospital.Controllers
{
    // Controlador de facturas: solo recibe la peticion y delega al servicio
    public class FacturaController(IFacturaService _facturaService, IPacienteService _pacienteService) : Controller
    {
        [Authorize(Policy = "factura.ver")]
        public async Task<IActionResult> Index()
        {
            ViewBag.Pacientes = await _pacienteService.GetAllAsync();
            return View();
        }
        [Authorize(Policy = "factura.ver")]
        [HttpPost]
        public async Task<IActionResult> GetData([FromForm] DataTableRequestDTO request)
        {
            var result = await _facturaService.GetPagedAsync(request);
            return Ok(result);
        }
        [Authorize(Policy = "factura.ver")]
        [HttpPost]
        public async Task<IActionResult> GetByid(int id)
        {
            var result = await _facturaService.GetByidAsync(id);
            if (result == null)
                return NotFound(new { success = false, message = "Factura no encontrada" });
            return Ok(result);
        }
        [Authorize(Policy = "factura.crear")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SaveFacturaDTO dTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Datos invalidos" });
            var (sucess, errorMessage) = await _facturaService.CreateAsync(dTO);
            return sucess ?
                Ok(new { success = true, message = "Factura creada correctamente" }) :
                BadRequest(new { success = false, message = errorMessage });
        }
        [Authorize(Policy = "factura.actualizar")]
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] SaveFacturaDTO dTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Datos invalidos" });
            var (sucess, errorMessage) = await _facturaService.UpDateAsync(dTO);
            return sucess ?
                Ok(new { success = true, message = "Factura actualizada correctamente" }) :
                BadRequest(new { success = false, message = errorMessage });
        }
        [Authorize(Policy = "factura.borrar")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var (sucess, errorMessage) = await _facturaService.DeleteAsync(id);
            return sucess ?
                Ok(new { success = true, message = "Factura eliminada correctamente" }) :
                BadRequest(new { success = false, message = errorMessage });
        }
    }
}
