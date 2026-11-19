using Hospital.Models;

namespace Hospital.Helpers
{
    public class EffectivePermission
    {
        public static IReadOnlyList<string> fromUser(Usuario usuario)
        {
            var fromRole = usuario.Role?.Rolepermissions?
                 .Select(rp => rp.Permission.name)
                 .ToHashSet(StringComparer.OrdinalIgnoreCase) ?? new HashSet<string>();

            var granted = usuario.UsuarioPermisos?
                .Where(up => up.isGranted)
                .Select(up => up.Permission.name)
                .ToList() ?? new List<string>();

            var denied = usuario.UsuarioPermisos?
                .Where(up => !up.isGranted)
                .Select(up => up.Permission.name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase) ?? new HashSet<string>();

            foreach (var permission in granted)
            {
                fromRole.Add(permission);
            }

            foreach (var permission in denied)
            {
                fromRole.Remove(permission);
            }

            return fromRole.OrderBy(p => p).ToList();
        }
    }
}

