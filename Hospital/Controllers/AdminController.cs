using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Hospital.DTOS;
using Hospital.Extensions;
using Hospital.Helpers;
using Hospital.Models;
using Hospital.Repository;
using Hospital.Service;

namespace Hospital.Controllers
{
    public class AdminController(IUsuarioRepository _usuarioRepository, IRolePermissionService _rolePermissionService) : Controller
    {
        private async Task<IReadOnlyList<string>> GetEffecctivePermissionAsync()
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
                return User.GetPermissions();
            var dbUser = await _usuarioRepository.GetUsuarioByIdwithPermissionAsync(userId);
            return dbUser != null
                ? EffectivePermission.fromUser(dbUser)
                : User.GetPermissions();

        }

        [Authorize(Policy = "admin.panel")]
        public IActionResult Index()
        {
            ViewBag.Permissions = User.GetPermissions();
            return View();
        }

        [Authorize(Policy = "permiso.ver")]
        [HttpPost]
        public async Task<IActionResult> GetPermissionData([FromForm] DataTableRequestDTO request)
        {
            var result = await _rolePermissionService.GetPermissionsPagedAsync(request);
            return Ok(result);
        }

        [Authorize(Policy = "permiso.ver")]
        public async Task<IActionResult> Permissions()
        {
            var effective = await GetEffecctivePermissionAsync();
            ViewBag.CanCreate = effective.Contains("permiso.crear", StringComparer.OrdinalIgnoreCase);
            ViewBag.CanUpdate = effective.Any(p =>
            string.Equals(p, "permiso.actualizar", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(p, "permiso.crear", StringComparison.OrdinalIgnoreCase));
            ViewBag.CanDelete = effective.Any(p =>
            string.Equals(p, "permiso.borrar", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(p, "permiso.crear", StringComparison.OrdinalIgnoreCase));
            return View();
        }

        [Authorize(Policy = "permiso.ver")]
        [HttpGet]
        public async Task<IActionResult> GetPermissionById(int id)
        {
            var permission = await _rolePermissionService.GetPermissionByIdAsync(id);
            if (permission == null)
                return NotFound(new { mesage = "Permiso no encontrado" });
            return Ok(permission);
        }

        [Authorize(Policy = "permiso.crear")]
        [HttpPost]
        public async Task<IActionResult> CreatePermission([FromBody] SavePermissionDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Revise los datos del formulario" });
            var (success, _, errorMessage) = await _rolePermissionService.CreatePermissionAsync(dto);
            if (!success)
                return BadRequest(new { success = false, message = errorMessage });
            return Ok(new { success = true, message = $"Permiso '{dto.name}' creado correctamente" });

        }

        [Authorize(Policy = "permiso.actualizar")]
        [HttpPost]
        public async Task<IActionResult> UpdatePermission([FromBody] SavePermissionDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Revise los datos del formulario" });
            var (success, errorMessage) = await _rolePermissionService.UpdatePermissionAsync(dto);
            if (!success)
                return BadRequest(new { success = false, message = errorMessage });
            return Ok(new { success = true, message = $"Permiso '{dto.name}' actualizado correctamente" });

        }

        [Authorize(Policy = "permiso.borrar")]
        [HttpPost]
        public async Task<IActionResult> DeletePermission(int id)
        {
            var (success, errorMessage) = await _rolePermissionService.DeletePermissionAsync(id);
            if (!success)
                return BadRequest(new { success = false, message = errorMessage });
            return Ok(new { success = true, message = $"Permiso borrado correctamente" });
        }

        [Authorize(Policy = "rol.ver")]
        public async Task<IActionResult> Roles()
        {
            var roles = await _rolePermissionService.GetAllRolesAsync();
            return View(roles);
        }

        [Authorize(Policy = "rol.crear")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRol(CreateRolDTO dto)
        {
            if (!ModelState.IsValid)
            {
                var roles = await _rolePermissionService.GetAllRolesAsync();
                ViewBag.CreateError = "El nombre del rol es obligatorio";
                return View("Roles", roles);
            }

            var (success, _, errorMessage) = await _rolePermissionService.CreateRoleAsync(dto);
            if (!success)
            {
                var roles = await _rolePermissionService.GetAllRolesAsync();
                ViewBag.CreateError = errorMessage;
                return View("Roles", roles);
            }
            TempData["Success"] = $"Role '{dto.name}' creado correctamento";
            return RedirectToAction(nameof(Roles));

        }

        [Authorize(Policy = "rol.asignar")]
        public async Task<IActionResult> RoleDetails(int id)
        {
            var role = await _rolePermissionService.GetRoleByIdAsync(id);
            if (role == null)
                return NotFound();
            var allPermission = await _rolePermissionService.GetAllPermissionsAsync();
            ViewBag.AllPermission = allPermission;
            return View(role);
        }

        [Authorize(Policy = "rol.asignar")]
        [HttpPost]
        public async Task<IActionResult> AssignPermissionToRole(int roleId, int permissionId)
        {
            var (success, errorMessage) = await _rolePermissionService.AssignPermissionToRoleAsync(roleId, permissionId);
            return Ok(new { success = true, message = success ? "Permiso asignado" : errorMessage });
        }

        [Authorize(Policy = "rol.asignar")]
        [HttpPost]
        public async Task<IActionResult> RemovePermissionFromRole(int roleId, int permissionId)
        {
            var (success, errorMessage) = await _rolePermissionService.RemovePermissionFromRoleAsync(roleId, permissionId);
            return Ok(new { success = true, message = success ? "Permiso eliminado" : errorMessage });
        }

        [Authorize(Policy = "usuario.ver")]
        public async Task<IActionResult> Users()
        {
            var users = await _rolePermissionService.GetAllUsersAsync();
            return View(users);
        }

        [Authorize(Policy = "usuario.asignar")]
        public async Task<IActionResult> UserPermission(int id)
        {
            var user = await _rolePermissionService.GetUserPermissionsAsync(id);
            if (user == null)
                return NotFound();
            var allPermission = await _rolePermissionService.GetAllPermissionsAsync();
            var allRoles = await _rolePermissionService.GetAllRolesAsync();
            ViewBag.AllPermission = allPermission;
            ViewBag.AllRoles = allRoles;
            ViewBag.CanChangeRole = User.HasPermission("usuario.cambiar_rol");
            return View(user);
        }

        [Authorize(Policy = "usuario.asignar")]
        [HttpPost]
        public async Task<IActionResult> AssignPermissionToUser(int userId, [FromBody] AssingUserPermission dto)
        {
            var (success, errorMessage) = await _rolePermissionService.AssignPermissionToUserAsync(userId, dto);
            return Json(new { success, message = success ? (dto.isgranted ? "Permiso concedido al usuario" : "Permiso denegado al usuario") : errorMessage });
        }

        [Authorize(Policy = "usuario.asignar")]
        [HttpPost]
        public async Task<IActionResult> RemoveUserPermission(int userId, int permissionId)
        {
            var (success, errorMessage) = await _rolePermissionService.RemoveUserPermissionAsync(userId, permissionId);
            return Json(new { success, message = success ? "Asignacion directa removida" : errorMessage });

        }

        [Authorize(Policy = "usuario.cambiar_rol")]
        [HttpPost]
        public async Task<IActionResult> UpdateUserRole(int userId, [FromBody] UpdateUserRolDTO dto)
        {
            var (success, errorMessage) = await _rolePermissionService.UpdateUserRoleAsync(userId, dto.RoleId);
            return Json(new { success, message = success ? "Rol actualizado" : errorMessage });
        }
    }
}
