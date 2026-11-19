using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Hospital.Extensions;
using Hospital.Helpers;
using Hospital.Repository;

namespace Hospital.Viecomponents
{
    public class NavigationViewComponent(IUsuarioRepository _usuarioRepository) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = HttpContext.User as ClaimsPrincipal;
            var permission = user?.GetPermissions() ?? [];
            if (user?.Identity?.IsAuthenticated == true && int.TryParse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id))

            {
                var dbUser = await _usuarioRepository.GetUsuarioByIdwithPermissionAsync(id);
                if (dbUser != null)
                    permission = EffectivePermission.fromUser(dbUser);


            }
            return View(permission);
        }

    }
}