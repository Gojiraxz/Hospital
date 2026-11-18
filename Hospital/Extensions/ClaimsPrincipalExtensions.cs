using System.Runtime.CompilerServices;
using System.Security.Claims;
using Hospital.Models;

namespace Hospital.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public const string permissionClaimType = "permission"; 
        
        public static bool HasPermission(this ClaimsPrincipal user, string permission)
        {
            return user.Claims.Any(c => c.Type == permissionClaimType 
            && string.Equals(c.Value,permission,StringComparison.OrdinalIgnoreCase));
        }
        public static bool HasAnyPermission(this ClaimsPrincipal user, params string[] permissions)
        {
            return permissions.Any(user.HasPermission); 
        }
        public static IReadOnlyList<string> GetPermissions(this ClaimsPrincipal user)
        {
            return user.Claims.Where(c => c.Type == permissionClaimType)
                .Select(c => c.Value)
                .OrderBy(p => p)
                .ToList();
        }
    }
}
