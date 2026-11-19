using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital.DTOS;
using Hospital.Service;

namespace Hospital.Controllers
{
    // Controlador de proveedores: solo recibe la peticion y delega al servicio
    public class ProveedorController(IProveedorService _proveedorService) : Controller
    {
        [Authorize(Policy = "proveedor.ver")]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Policy = "proveedor.ver")]
        [HttpPost]
        public async Task<IActionResult> GetData([FromForm] DataTableRequestDTO request)
        {
            var result = await _proveedorService.GetPagedAsync(request);
            return Ok(result);
        }
        [Authorize(Policy = "proveedor.ver")]
        [HttpPost]
        public async Task<IActionResult> GetByid(int id)
        {
            var result = await _proveedorService.GetByidAsync(id);
            if (result == null)
                return NotFound(new { success = false, message = "Proveedor no encontrado" });
            return Ok(result);
        }
        [Authorize(Policy = "proveedor.crear")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SaveProveedorDTO dTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Datos invalidos" });
            var (sucess, errorMessage) = await _proveedorService.CreateAsync(dTO);
            return sucess ?
                Ok(new { success = true, message = "Proveedor creado correctamente" }) :
                BadRequest(new { success = false, message = errorMessage });
        }
        [Authorize(Policy = "proveedor.actualizar")]
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] SaveProveedorDTO dTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Datos invalidos" });
            var (sucess, errorMessage) = await _proveedorService.UpDateAsync(dTO);
            return sucess ?
                Ok(new { success = true, message = "Proveedor actualizado correctamente" }) :
                BadRequest(new { success = false, message = errorMessage });
        }
        [Authorize(Policy = "proveedor.borrar")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var (sucess, errorMessage) = await _proveedorService.DeleteAsync(id);
            return sucess ?
                Ok(new { success = true, message = "Proveedor eliminado correctamente" }) :
                BadRequest(new { success = false, message = errorMessage });
        }
    }
}
