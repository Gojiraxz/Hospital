using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital.DTOS;
using Hospital.Service;

namespace Hospital.Controllers
{
    public class AuthController(IAuthService authService, ILogger<AuthController> logger, IWebHostEnvironment env) : Controller
    {
        private readonly IAuthService _authService = authService;
        private readonly ILogger<AuthController> _logger = logger;
        private readonly IWebHostEnvironment _env = env;

        // Pantalla de inicio: si no existe ningun admin muestra "Crear cuenta de administrador"
        [HttpGet]
        public async Task<IActionResult> Login()
        {
            var token = Request.Cookies["access_token"];
            if (!string.IsNullOrEmpty(token))
                return RedirectToAction("Index", "Home");
            ViewBag.NeedsSetup = !await _authService.AdminExistsAsync();
            return View();
        }

        // Crea la primera cuenta de administrador (solo funciona mientras no exista ninguna)
        [HttpPost]
        public async Task<IActionResult> SetupAdmin([FromBody] SetupAdminDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, errorMessage = "Datos invalidos. Usuario minimo 3 caracteres, correo valido y contraseña de al menos 8 caracteres." });
            try
            {
                var (success, error) = await _authService.SetupAdminAsync(dto);
                if (!success)
                    return Conflict(new { success = false, errorMessage = error });
                return Ok(new { success = true, message = "Cuenta de administrador creada" });
            }
            catch (Exception ex)
            {
                return ErrorInterno(ex, "crear la cuenta de administrador");
            }
        }

        // Inicio de sesion del admin/doctor: nombre de usuario + contraseña
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, errorMessage = "Datos invalidos" });
            try
            {
                var (success, result, error) = await _authService.LoginAsync(dto);
                if (!success || result is null)
                    return Unauthorized(new { success = false, errorMessage = error ?? "Credenciales invalidas" });
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = result.ExpiresAt
                };
                Response.Cookies.Append("access_token", result.accessToken, cookieOptions);
                var refreshOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = result.RefreshTokenExpiresAt
                };
                Response.Cookies.Append("refresh_token", result.RefreshToken, refreshOptions);
                return Ok(new { success = true, message = "Login Exitoso!", redirect = "/Home/Index" });
            }
            catch (Exception ex)
            {
                return ErrorInterno(ex, "iniciar sesion");
            }
        }

        // Inicio de sesion del paciente: solo con la cedula que registro el admin
        [HttpPost]
        public async Task<IActionResult> PacienteLogin([FromBody] PacienteLoginDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, errorMessage = "Ingresa tu numero de cédula" });
            try
            {
                var (success, result, error) = await _authService.LoginPacienteAsync(dto);
                if (!success || result is null)
                    return Unauthorized(new { success = false, errorMessage = error ?? "Cédula no registrada" });
                Response.Cookies.Append("access_token", result.accessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = result.ExpiresAt
                });
                // Por si quedo una sesion de personal en este navegador
                Response.Cookies.Delete("refresh_token");
                return Ok(new { success = true, message = "Bienvenido", redirect = "/Portal" });
            }
            catch (Exception ex)
            {
                return ErrorInterno(ex, "iniciar sesion como paciente");
            }
        }

        // Registra el detalle en la consola del servidor y devuelve un JSON claro al navegador
        private IActionResult ErrorInterno(Exception ex, string accion)
        {
            _logger.LogError(ex, "Error al {Accion}", accion);
            var detalle = _env.IsDevelopment() ? $" Detalle: {ex.GetType().Name}: {ex.Message}" : string.Empty;
            return StatusCode(500, new
            {
                success = false,
                errorMessage = $"Error interno del servidor al {accion}.{detalle}"
            });
        }

        [Authorize]
        [HttpPost]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("access_token");
            Response.Cookies.Delete("refresh_token");
            return Json(new { success = true, message = "sesion cerrada exitosamente" });

        }
        [HttpGet]
        public async Task<IActionResult> confirmEmail(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
                return BadRequest("parametros invalidos");
            var confirmado = await _authService.ConfirmEmailAsync(email, token);
            if (confirmado)
            {
                TempData["success"] = "Correo confirmado ya puedes iniciar session";
                return RedirectToAction("Login");
            }
            TempData["error"] = "El token es invalido o ya expiro";
            return RedirectToAction("Login");
        }
        [HttpGet]

        public IActionResult ForgetPassword() => RedirectToAction("Login");

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordRequestDTO model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.email))
            {
                return BadRequest(new { success = false, errorMessage = "El correo es requerido" });
            }
            var sent = await _authService.SendResetPassswordLinkAsync(model.email);
            if (sent)
            {
                return Ok(new { success = true });
            }
            else
            {
                return NotFound(new { success = false, errorMessage = "Correo no encontrado" });
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string email, string token)
        {
            var model = new ResetPasswordRequestDTO { email = email, token = token };
            return View(model);
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDTO dTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Datos invalidos", errorMessage = "Datos invalidos" });
            var success = await _authService.ResetPasswordAsync(dTO);
            if (success)
                return Ok(new { success = true });
            return BadRequest(new { success = false, message = "Token invalido o expirado", errorMessage = "Token invalido o expirado" });
        }
        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword() => View();
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO dTO)
        {
            if (!ModelState.IsValid)
            {
                return View(dTO);
            }
            var UserName = User.Identity?.Name!;
            var success = await _authService.ChangePassWordAsync(UserName, dTO);
            if (success)
            {
                Response.Cookies.Delete("access_token");
                Response.Cookies.Delete("refresh_token");
                TempData["Success"] = "Contraseña  actualizada";
                return RedirectToAction("Login");
            }
            ModelState.AddModelError("", "Contraseña actual incorrecta");
            return View(dTO);
        }
        [HttpPost]
        public async Task<IActionResult> refreshToken()
        {
            var refreshToken = Request.Cookies["refresh_token"];
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized();
            var newToken = await _authService.RefreshAcessTokenAsync(refreshToken);
            if (newToken == null)
                return Unauthorized();

            var accessOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = newToken.ExpiresAt
            };
            var RefreshOption = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("access_token", newToken.accessToken, accessOptions);
            Response.Cookies.Append("refresh_token", newToken.RefreshToken, RefreshOption);
            return Ok();

        }
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View("AccessDenied");
        }
    }
}
