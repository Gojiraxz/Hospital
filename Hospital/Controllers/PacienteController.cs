using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital.DTOS;
using Hospital.Service;

namespace Hospital.Controllers
{
    // Controlador de pacientes: solo recibe la peticion y delega al servicio
    public class PacienteController(IPacienteService _pacienteService) : Controller
    {
        [Authorize(Policy = "paciente.ver")]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Policy = "paciente.ver")]
        [HttpPost]
        public async Task<IActionResult> GetData([FromForm] DataTableRequestDTO request)
        {
            var result = await _pacienteService.GetPagedAsync(request);
            return Ok(result);
        }
        [Authorize(Policy = "paciente.ver")]
        [HttpPost]
        public async Task<IActionResult> GetByid(int id)
        {
            var result = await _pacienteService.GetByidAsync(id);
            if (result == null)
                return NotFound(new { success = false, message = "Paciente no encontrado" });
            return Ok(result);
        }
        [Authorize(Policy = "paciente.crear")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SavePacienteDTO dTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Datos invalidos" });
            var (sucess, errorMessage) = await _pacienteService.CreateAsync(dTO);
            return sucess ?
                Ok(new { success = true, message = "Paciente creado correctamente" }) :
                BadRequest(new { success = false, message = errorMessage });
        }
        [Authorize(Policy = "paciente.actualizar")]
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] SavePacienteDTO dTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Datos invalidos" });
            var (sucess, errorMessage) = await _pacienteService.UpDateAsync(dTO);
            return sucess ?
                Ok(new { success = true, message = "Paciente actualizado correctamente" }) :
                BadRequest(new { success = false, message = errorMessage });
        }
        [Authorize(Policy = "paciente.borrar")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var (sucess, errorMessage) = await _pacienteService.DeleteAsync(id);
            return sucess ?
                Ok(new { success = true, message = "Paciente eliminado correctamente" }) :
                BadRequest(new { success = false, message = errorMessage });
        }
    }
}
