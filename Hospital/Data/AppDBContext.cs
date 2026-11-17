using Microsoft.EntityFrameworkCore;
using Hospital.Models;

namespace Hospital.Data
{
    public class AppDBcontext(DbContextOptions<AppDBcontext> options) : DbContext(options)
    {
        public DbSet<Role> roles { get; set; } 
        public DbSet <permission> permissions { get; set; }

        public DbSet <Usuario> usuarios { get; set; }
        public DbSet <UsuarioPermiso> usuariospermisos { get; set; }
        public DbSet<Rolepermission> rolespermisos { get; set; }

        public DbSet<Especialidad> especialidades { get; set; }
        public DbSet<Doctor> doctores { get; set; }
        public DbSet<Enfermero> enfermeros { get; set; }
        public DbSet<Turno> turnos { get; set; }
        public DbSet<Habitacion> habitaciones { get; set; }
        public DbSet<Paciente> pacientes { get; set; }
        public DbSet<Proveedor> proveedores { get; set; }
        public DbSet<Medicamento> medicamentos { get; set; }
        public DbSet<Laboratorio> laboratorios { get; set; }
        public DbSet<Cita> citas { get; set; }
        public DbSet<Receta> recetas { get; set; }
        public DbSet<ConsultaMedica> consultasmedicas { get; set; }
        public DbSet<ExamenMedico> examenesmedicos { get; set; }
        public DbSet<Cirugia> cirugias { get; set; }
        public DbSet<Factura> facturas { get; set; }
        public DbSet<DetalleFactura> detallesfactura { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Role)
                .WithMany(r => r.usuario)
                .HasForeignKey(u => u.RolId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Rolepermission>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });
            modelBuilder.Entity<Rolepermission>()
                .HasOne(rp => rp.Role)
                .WithMany(rp => rp.Rolepermissions)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UsuarioPermiso>()
                .HasKey(up => new { up.Usuarioid, up.PermissionId });
            modelBuilder.Entity<UsuarioPermiso>()
                .HasOne(up => up.Usuario)
                .WithMany(u => u.UsuarioPermisos)
                .HasForeignKey(up => up.Usuarioid)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<UsuarioPermiso>()
                .HasOne(rp => rp.Permission)
                .WithMany(rp => rp.Permissions)
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Doctor>()
                .HasOne(x => x.Especialidad)
                .WithMany(p => p.Doctores)
                .HasForeignKey(x => x.EspecialidadId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Enfermero>()
                .HasOne(x => x.Especialidad)
                .WithMany(p => p.Enfermeros)
                .HasForeignKey(x => x.EspecialidadId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Turno>()
                .HasOne(x => x.Enfermero)
                .WithMany(p => p.Turnos)
                .HasForeignKey(x => x.EnfermeroId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Turno>()
                .HasOne(x => x.Doctor)
                .WithMany(p => p.Turnos)
                .HasForeignKey(x => x.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Medicamento>()
                .HasOne(x => x.Proveedor)
                .WithMany(p => p.Medicamentos)
                .HasForeignKey(x => x.ProveedorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Medicamento>().Property(x => x.Precio).HasPrecision(18, 2);

            modelBuilder.Entity<Cita>()
                .HasOne(x => x.Doctor)
                .WithMany(p => p.Citas)
                .HasForeignKey(x => x.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cita>()
                .HasOne(x => x.Paciente)
                .WithMany(p => p.Citas)
                .HasForeignKey(x => x.PacienteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Habitacion>()
                .HasOne(x => x.Paciente)
                .WithMany(p => p.Habitaciones)
                .HasForeignKey(x => x.PacienteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Receta>()
                .HasOne(x => x.Doctor)
                .WithMany(p => p.Recetas)
                .HasForeignKey(x => x.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Receta>()
                .HasOne(x => x.Paciente)
                .WithMany(p => p.Recetas)
                .HasForeignKey(x => x.PacienteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ConsultaMedica>()
                .HasOne(x => x.Doctor)
                .WithMany(p => p.ConsultasMedicas)
                .HasForeignKey(x => x.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ConsultaMedica>()
                .HasOne(x => x.Cita)
                .WithMany(p => p.ConsultasMedicas)
                .HasForeignKey(x => x.CitaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ConsultaMedica>()
                .HasOne(x => x.Paciente)
                .WithMany(p => p.ConsultasMedicas)
                .HasForeignKey(x => x.PacienteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ExamenMedico>()
                .HasOne(x => x.ConsultaMedica)
                .WithMany(p => p.ExamenesMedicos)
                .HasForeignKey(x => x.ConsultaMedicaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ExamenMedico>()
                .HasOne(x => x.Laboratorio)
                .WithMany(p => p.ExamenesMedicos)
                .HasForeignKey(x => x.LaboratorioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cirugia>()
                .HasOne(x => x.Doctor)
                .WithMany(p => p.Cirugias)
                .HasForeignKey(x => x.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cirugia>()
                .HasOne(x => x.Habitacion)
                .WithMany(p => p.Cirugias)
                .HasForeignKey(x => x.HabitacionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cirugia>()
                .HasOne(x => x.Paciente)
                .WithMany(p => p.Cirugias)
                .HasForeignKey(x => x.PacienteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Factura>()
                .HasOne(x => x.Paciente)
                .WithMany(p => p.Facturas)
                .HasForeignKey(x => x.PacienteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Factura>().Property(x => x.Total).HasPrecision(18, 2);

            modelBuilder.Entity<DetalleFactura>()
                .HasOne(x => x.Factura)
                .WithMany(p => p.DetallesFactura)
                .HasForeignKey(x => x.FacturaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DetalleFactura>()
                .HasOne(x => x.Medicamento)
                .WithMany(p => p.DetallesFactura)
                .HasForeignKey(x => x.MedicamentoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DetalleFactura>().Property(x => x.PrecioUnitario).HasPrecision(18, 2);

            modelBuilder.Entity<DetalleFactura>().Property(x => x.Subtotal).HasPrecision(18, 2);
        }
    }
}
