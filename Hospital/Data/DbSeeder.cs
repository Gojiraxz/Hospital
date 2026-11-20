using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Hospital.Models;

namespace Hospital.Data
{
    public class DbSeeder
    {
        public static async Task seedAsync(AppDBcontext context)
        {
           // Crea la base y las tablas si no existen (no requiere migraciones)
            await context.Database.EnsureCreatedAsync();
            try
            {
                await context.roles.AnyAsync();
            }
            catch (Microsoft.Data.SqlClient.SqlException)
            {
                // La base existe pero esta vacia (sin tablas): se crean las tablas
                await context.GetService<IRelationalDatabaseCreator>().CreateTablesAsync();
            }
            await UpgradeSchemaAsync(context);
            await SeedRolesAsync(context);
            await SeedPermissionAsync(context);
            await SeedRolePermissionAsync(context);
            await SeedHospitalAsync(context);
        }
        // Agrega columnas nuevas a bases creadas con versiones anteriores
        private static async Task UpgradeSchemaAsync(AppDBcontext context)
        {
            await context.Database.ExecuteSqlRawAsync(@"
SI OBJECT_ID('habitaciones') NO ES NULO Y COL_LENGTH('habitaciones', 'PacienteId') ES NULO
COMENZAR
    ALTER TABLE habitaciones ADD PacienteId int NULL;
    ALTER TABLE habitaciones ADD CONSTRAINT FK_habitaciones_pacientes_PacienteId FOREIGN KEY (PacienteId) REFERENCIAS pacientes (Id);
    CREAR ÍNDICE IX_habitaciones_PacienteId ON habitaciones (PacienteId);
FIN"");");
        }
        private static async Task SeedRolesAsync(AppDBcontext context)
        {
            var changed = false;
            if (!await context.roles.AnyAsync(r => r.name == "Admin"))
            {
                context.roles.Add(new Role { name = "Admin" });
                changed = true;


            }
            if (!await context.roles.AnyAsync(r => r.name == "User"))
            {
                context.roles.Add(new Role { name = "User" });
                changed = true;
            }
            // Rol clinico para demostrar la asignacion de permisos a otro rol
            if (!await context.roles.AnyAsync(r => r.name == "Doctor"))
            {
                context.roles.Add(new Role { name = "Doctor" });
                changed = true;
            }
            if (changed)
            {
                await context.SaveChangesAsync();
            }
        }
        private static async Task SeedPermissionAsync(AppDBcontext context)
        {
            var nuevos = new List<permission>
            {
                new permission { name = "especialidad.ver", Description = "Ver especialidades" },
                new permission { name = "especialidad.crear", Description = "Crear especialidad" },
                new permission { name = "especialidad.actualizar", Description = "Actualizar especialidad" },
                new permission { name = "especialidad.borrar", Description = "Borrar especialidad" },
                new permission { name = "doctor.ver", Description = "Ver doctores" },
                new permission { name = "doctor.crear", Description = "Crear doctor" },
                new permission { name = "doctor.actualizar", Description = "Actualizar doctor" },
                new permission { name = "doctor.borrar", Description = "Borrar doctor" },
                new permission { name = "enfermero.ver", Description = "Ver enfermeros" },
                new permission { name = "enfermero.crear", Description = "Crear enfermero" },
                new permission { name = "enfermero.actualizar", Description = "Actualizar enfermero" },
                new permission { name = "enfermero.borrar", Description = "Borrar enfermero" },
                new permission { name = "turno.ver", Description = "Ver turnos" },
                new permission { name = "turno.crear", Description = "Crear turno" },
                new permission { name = "turno.actualizar", Description = "Actualizar turno" },
                new permission { name = "turno.borrar", Description = "Borrar turno" },
                new permission { name = "habitacion.ver", Description = "Ver habitaciones" },
                new permission { name = "habitacion.crear", Description = "Crear habitación" },
                new permission { name = "habitacion.actualizar", Description = "Actualizar habitación" },
                new permission { name = "habitacion.borrar", Description = "Borrar habitación" },
                new permission { name = "paciente.ver", Description = "Ver pacientes" },
                new permission { name = "paciente.crear", Description = "Crear paciente" },
                new permission { name = "paciente.actualizar", Description = "Actualizar paciente" },
                new permission { name = "paciente.borrar", Description = "Borrar paciente" },
                new permission { name = "proveedor.ver", Description = "Ver proveedores" },
                new permission { name = "proveedor.crear", Description = "Crear proveedor" },
                new permission { name = "proveedor.actualizar", Description = "Actualizar proveedor" },
                new permission { name = "proveedor.borrar", Description = "Borrar proveedor" },
                new permission { name = "medicamento.ver", Description = "Ver medicamentos" },
                new permission { name = "medicamento.crear", Description = "Crear medicamento" },
                new permission { name = "medicamento.actualizar", Description = "Actualizar medicamento" },
                new permission { name = "medicamento.borrar", Description = "Borrar medicamento" },
                new permission { name = "laboratorio.ver", Description = "Ver laboratorios" },
                new permission { name = "laboratorio.crear", Description = "Crear laboratorio" },
                new permission { name = "laboratorio.actualizar", Description = "Actualizar laboratorio" },
                new permission { name = "laboratorio.borrar", Description = "Borrar laboratorio" },
                new permission { name = "cita.ver", Description = "Ver citas" },
                new permission { name = "cita.crear", Description = "Crear cita" },
                new permission { name = "cita.actualizar", Description = "Actualizar cita" },
                new permission { name = "cita.borrar", Description = "Borrar cita" },
                new permission { name = "consulta.ver", Description = "Ver consultas médicas" },
                new permission { name = "consulta.crear", Description = "Crear consulta médica" },
                new permission { name = "consulta.actualizar", Description = "Actualizar consulta médica" },
                new permission { name = "consulta.borrar", Description = "Borrar consulta médica" },
                new permission { name = "examen.ver", Description = "Ver exámenes médicos" },
                new permission { name = "examen.crear", Description = "Crear examen médico" },
                new permission { name = "examen.actualizar", Description = "Actualizar examen médico" },
                new permission { name = "examen.borrar", Description = "Borrar examen médico" },
                new permission { name = "cirugia.ver", Description = "Ver cirugías" },
                new permission { name = "cirugia.crear", Description = "Crear cirugía" },
                new permission { name = "cirugia.actualizar", Description = "Actualizar cirugía" },
                new permission { name = "cirugia.borrar", Description = "Borrar cirugía" },
                new permission { name = "factura.ver", Description = "Ver facturas" },
                new permission { name = "factura.crear", Description = "Crear factura" },
                new permission { name = "factura.actualizar", Description = "Actualizar factura" },
                new permission { name = "factura.borrar", Description = "Borrar factura" },
                new permission { name = "detallefactura.ver", Description = "Ver detalle de facturas" },
                new permission { name = "detallefactura.crear", Description = "Crear detalle de factura" },
                new permission { name = "detallefactura.actualizar", Description = "Actualizar detalle de factura" },
                new permission { name = "detallefactura.borrar", Description = "Borrar detalle de factura" },
                new permission { name = "rol.ver", Description = "Ver roles" },
                new permission { name = "rol.crear", Description = "Crear rol" },
                new permission { name = "rol.asignar", Description = "Asignar permisos a rol" },
                new permission { name = "permiso.ver", Description = "Ver permisos" },
                new permission { name = "permiso.crear", Description = "Crear permiso" },
                new permission { name = "permiso.actualizar", Description = "Actualizar permiso" },
                new permission { name = "permiso.borrar", Description = "Borrar permiso" },
                new permission { name = "usuario.ver", Description = "Ver usuarios" },
                new permission { name = "usuario.crear", Description = "Crear usuario" },
                new permission { name = "usuario.asignar", Description = "Asignar permisos a usuario" },
                new permission { name = "usuario.cambiar_rol", Description = "Cambiar rol de usuario" },
                new permission { name = "receta.ver", Description = "Ver solicitudes de recetas" },
                new permission { name = "receta.actualizar", Description = "Responder solicitudes de recetas" }
            };
            // Solo agrega los permisos que todavia no existen
            var existentes = await context.permissions.Select(p => p.name).ToListAsync();
            context.permissions.AddRange(nuevos.Where(n => !existentes.Contains(n.name)));
            await context.SaveChangesAsync();

        }
        private static async Task SeedRolePermissionAsync(AppDBcontext context)
        {
            if (await context.rolespermisos.AnyAsync())
            {
                // Base existente: el Admin recibe los permisos nuevos (por ejemplo receta.*)
                var admin = await context.roles.FirstAsync(r => r.name == "Admin");
                var yaTiene = await context.rolespermisos.Where(rp => rp.RoleId == admin.Id).Select(rp => rp.PermissionId).ToListAsync();
                var faltan = await context.permissions.Where(p => !yaTiene.Contains(p.Id)).ToListAsync();
                foreach (var nuevoPermiso in faltan)
                    context.rolespermisos.Add(new Rolepermission { RoleId = admin.Id, PermissionId = nuevoPermiso.Id });
                if (faltan.Count > 0)
                    await context.SaveChangesAsync();
                return;
            }
            var adminRole = await context.roles.FirstAsync(r => r.name == "Admin");
            var usuarioRole = await context.roles.FirstAsync(r => r.name == "User");
            var doctorRole = await context.roles.FirstAsync(r => r.name == "Doctor");
            var permissions = await context.permissions.ToListAsync();

            // Admin: todos los permisos
            foreach (var permission in permissions)
            {
                context.rolespermisos.Add(new Rolepermission
                {
                    RoleId = adminRole.Id,
                    PermissionId = permission.Id
                });
            }
            // User (recepcion): solo consulta pacientes, citas, doctores y especialidades
            var userPermissions = permissions.Where(p => p.name is "paciente.ver" or "cita.ver" or "doctor.ver" or "especialidad.ver")
                .ToList(); 
            foreach (var permission in userPermissions)
            {
                context.rolespermisos.Add(new Rolepermission
                {
                    RoleId = usuarioRole.Id,
                    PermissionId = permission.Id
                });
            }
            // Doctor: atiende pacientes, citas, consultas, examenes y cirugias (sin borrar)
            var doctorPermissions = permissions.Where(p =>
                    p.name is "paciente.ver" or "paciente.crear" or "paciente.actualizar"
                    or "cita.ver" or "cita.crear" or "cita.actualizar"
                    or "consulta.ver" or "consulta.crear" or "consulta.actualizar"
                    or "examen.ver" or "examen.crear" or "examen.actualizar"
                    or "cirugia.ver" or "cirugia.crear" or "cirugia.actualizar"
                    or "receta.ver" or "receta.actualizar"
                    or "doctor.ver" or "especialidad.ver" or "laboratorio.ver" or "habitacion.ver")
                .ToList();
            foreach (var permission in doctorPermissions)
            {
                context.rolespermisos.Add(new Rolepermission
                {
                    RoleId = doctorRole.Id,
                    PermissionId = permission.Id
                });
            }
            await context.SaveChangesAsync();
        }
        // Catalogos iniciales. No se crean doctores, pacientes ni usuarios de ejemplo:
        // los doctores y pacientes los registra el administrador manualmente.
        private static async Task SeedHospitalAsync(AppDBcontext context)
        {
            if (await context.especialidades.AnyAsync())
                return;

            context.especialidades.AddRange(
                new Especialidad { Nombre = "Cardiología" },
                new Especialidad { Nombre = "Pediatría" },
                new Especialidad { Nombre = "Cirugía General" },
                new Especialidad { Nombre = "Medicina Interna" },
                new Especialidad { Nombre = "Ginecología" });
            context.habitaciones.AddRange(
                new Habitacion { Numero = "101", Estado = "Disponible" },
                new Habitacion { Numero = "102", Estado = "Disponible" },
                new Habitacion { Numero = "201", Estado = "Disponible" });
            await context.SaveChangesAsync();
        }
    }
}
