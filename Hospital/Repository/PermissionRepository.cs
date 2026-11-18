using Microsoft.EntityFrameworkCore;
using Hospital.Data;
using Hospital.DTOS;
using Hospital.Models;

namespace Hospital.Repository
{
    public class PermissionRepository(AppDBcontext context) : IpermissionRepository
    {
        public async Task<permission?> CreateAsync(string name, string? description)
        {
            var nuevo = new permission
            {
                name = name.Trim().ToLowerInvariant(),
                Description = description?.Trim()
            };
            var entry = await context.permissions.AddAsync(nuevo);
            await context.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existente = await context.permissions.FindAsync(id);
            if (existente == null) return false;
            context.permissions.Remove(existente);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<IReadOnlyList<permission>> GetAllAsync()
        {
            return await context.permissions.OrderBy(n => n.name).ToListAsync();
        }

        public async Task<permission?> GetByIdAsync(int id)
        {
            return await context.permissions.FindAsync(id);
        }

        public async Task<permission?> GetByNameAsync(string name)
        {
            return await context.permissions.FirstOrDefaultAsync(x => x.name == name);
        }

        public async Task<datatableResponseDTO<PermissionDTO>> GetPagedAsync(DataTableRequestDTO request)
        {
            var query = context.permissions.AsQueryable();
            var recordsTotal = await query.CountAsync();

            var search = request.search?.value?.Trim();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.name.Contains(search)
                    || (p.Description != null && p.Description.Contains(search)));
            }
            var recordsFiltered = await query.CountAsync();

            var order = request.order.FirstOrDefault();
            var desc = string.Equals(order?.dir, "desc", StringComparison.OrdinalIgnoreCase);
            var columnName = request.columns.ElementAtOrDefault(order?.column ?? 0)?.data ?? "name";

            query = columnName.ToLowerInvariant() switch
            {
                "description" or "descripcion" => desc ? query.OrderByDescending(p => p.Description) : query.OrderBy(p => p.Description),
                "id" => desc ? query.OrderByDescending(p => p.Id) : query.OrderBy(p => p.Id),
                _ => desc ? query.OrderByDescending(p => p.name) : query.OrderBy(p => p.name)
            };

            var data = await query
                .Skip(request.start)
                .Take(request.length > 0 ? request.length : 10)
                .Select(p => new PermissionDTO
                {
                    Id = p.Id,
                    Name = p.name,
                    Description = p.Description
                })
                .ToListAsync();

            return new datatableResponseDTO<PermissionDTO>
            {
                draw = request.draw,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsFiltered,
                data = data
            };
        }

        public async Task<bool> IsAsingnedToUsuarioAsync(int permissionId)
        {
            return await context.usuariospermisos.AnyAsync(up => up.PermissionId == permissionId);
        }

        public async Task<bool> UpdateAsync(permission permission)
        {
            context.permissions.Update(permission);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
