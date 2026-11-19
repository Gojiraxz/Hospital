using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital.DTOS;
using Hospital.Service;

namespace Hospital.Controllers
{
    // Controlador de detalle de facturas: solo recibe la peticion y delega al servicio
    public class DetalleFacturaController(IDetalleFacturaService _detalleFacturaService, IFacturaService _facturaService, IMedicamentoService _medicamentoService) : Controller
    {
        [Authorize(Policy = "detallefactura.ver")]
        public async Task<IActionResult> Index()
        {
            ViewBag.Facturas = await _facturaService.GetAllAsync();
            ViewBag.Medicamentos = await _medicamentoService.GetAllAsync();
            return View();
        }
        [Authorize(Policy = "detallefactura.ver")]
        [HttpPost]
        public async Task<IActionResult> GetData([FromForm] DataTableRequestDTO request)
        {
            var result = await _detalleFacturaService.GetPagedAsync(request);
            return Ok(result);
        }
        [Authorize(Policy = "detallefactura.ver")]
        [HttpPost]
        public async Task<IActionResult> GetByid(int id)
        {
            var result = await _detalleFacturaService.GetByidAsync(id);
            if (result == null)
                return NotFound(new { success = false, message = "Detalle de factura no encontrado" });
            return Ok(result);
        }
        [Authorize(Policy = "detallefactura.crear")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SaveDetalleFacturaDTO dTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Datos invalidos" });
            var (sucess, errorMessage) = await _detalleFacturaService.CreateAsync(dTO);
            return sucess ?
                Ok(new { success = true, message = "Detalle de factura creado correctamente" }) :
                BadRequest(new { success = false, message = errorMessage });
        }
        [Authorize(Policy = "detallefactura.actualizar")]
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] SaveDetalleFacturaDTO dTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Datos invalidos" });
            var (sucess, errorMessage) = await _detalleFacturaService.UpDateAsync(dTO);
            return sucess ?
                Ok(new { success = true, message = "Detalle de factura actualizado correctamente" }) :
                BadRequest(new { success = false, message = errorMessage });
        }
        [Authorize(Policy = "detallefactura.borrar")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var (sucess, errorMessage) = await _detalleFacturaService.DeleteAsync(id);
            return sucess ?
                Ok(new { success = true, message = "Detalle de factura eliminado correctamente" }) :
                BadRequest(new { success = false, message = errorMessage });
        }
    }
}
