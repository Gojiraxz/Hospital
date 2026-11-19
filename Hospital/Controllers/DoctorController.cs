using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital.DTOS;
using Hospital.Service;

namespace Hospital.Controllers
{
    // Controlador de doctores: solo recibe la peticion y delega al servicio
    public class DoctorController(IDoctorService _doctorService, IEspecialidadService _especialidadService) : Controller
    {
        [Authorize(Policy = "doctor.ver")]
        public async Task<IActionResult> Index()
        {
            ViewBag.Especialidades = await _especialidadService.GetAllAsync();
            return View();
        }
        [Authorize(Policy = "doctor.ver")]
        [HttpPost]
        public async Task<IActionResult> GetData([FromForm] DataTableRequestDTO request)
        {
            var result = await _doctorService.GetPagedAsync(request);
            return Ok(result);
        }
        [Authorize(Policy = "doctor.ver")]
        [HttpPost]
        public async Task<IActionResult> GetByid(int id)
        {
            var result = await _doctorService.GetByidAsync(id);
            if (result == null)
                return NotFound(new { success = false, message = "Doctor no encontrado" });
            return Ok(result);
        }
        [Authorize(Policy = "doctor.crear")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SaveDoctorDTO dTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Datos invalidos" });
            var (sucess, errorMessage) = await _doctorService.CreateAsync(dTO);
            return sucess ?
                Ok(new { success = true, message = "Doctor creado correctamente" }) :
                BadRequest(new { success = false, message = errorMessage });
        }
        [Authorize(Policy = "doctor.actualizar")]
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] SaveDoctorDTO dTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Datos invalidos" });
            var (sucess, errorMessage) = await _doctorService.UpDateAsync(dTO);
            return sucess ?
                Ok(new { success = true, message = "Doctor actualizado correctamente" }) :
                BadRequest(new { success = false, message = errorMessage });
        }
        [Authorize(Policy = "doctor.borrar")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var (sucess, errorMessage) = await _doctorService.DeleteAsync(id);
            return sucess ?
                Ok(new { success = true, message = "Doctor eliminado correctamente" }) :
                BadRequest(new { success = false, message = errorMessage });
        }
    }
}
