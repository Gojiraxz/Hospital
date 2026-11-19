using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital.DTOS;
using Hospital.Service;

namespace Hospital.Controllers
{
    // Controlador de medicamentos: solo recibe la peticion y delega al servicio
    public class MedicamentoController(IMedicamentoService _medicamentoService, IProveedorService _proveedorService) : Controller
    {
        [Authorize(Policy = "medicamento.ver")]
        public async Task<IActionResult> Index()
        {
            ViewBag.Proveedores = await _proveedorService.GetAllAsync();
            return View();
        }
        [Authorize(Policy = "medicamento.ver")]
        [HttpPost]
        public async Task<IActionResult> GetData([FromForm] DataTableRequestDTO request)
        {
            var result = await _medicamentoService.GetPagedAsync(request);
            return Ok(result);
        }
        [Authorize(Policy = "medicamento.ver")]
        [HttpPost]
        public async Task<IActionResult> GetByid(int id)
        {
            var result = await _medicamentoService.GetByidAsync(id);
            if (result == null)
                return NotFound(new { success = false, message = "Medicamento no encontrado" });
            return Ok(result);
        }
        [Authorize(Policy = "medicamento.crear")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SaveMedicamentoDTO dTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Datos invalidos" });
            var (sucess, errorMessage) = await _medicamentoService.CreateAsync(dTO);
            return sucess ?
                Ok(new { success = true, message = "Medicamento creado correctamente" }) :
                BadRequest(new { success = false, message = errorMessage });
        }
        [Authorize(Policy = "medicamento.actualizar")]
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] SaveMedicamentoDTO dTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Datos invalidos" });
            var (sucess, errorMessage) = await _medicamentoService.UpDateAsync(dTO);
            return sucess ?
                Ok(new { success = true, message = "Medicamento actualizado correctamente" }) :
                BadRequest(new { success = false, message = errorMessage });
        }
        [Authorize(Policy = "medicamento.borrar")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var (sucess, errorMessage) = await _medicamentoService.DeleteAsync(id);
            return sucess ?
                Ok(new { success = true, message = "Medicamento eliminado correctamente" }) :
                BadRequest(new { success = false, message = errorMessage });
        }
    }
}
