using Hospital.DTOS;
using Hospital.Models;
using Hospital.Repository;

namespace Hospital.Service
{
    // Logica de negocio y validaciones: Paciente
    public class PacienteService(IPacienteRepository repo) : IPacienteService
    {
        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(SavePacienteDTO dTO)
        {
            if (await repo.CedulaExisteAsync(dTO.cedula.Trim().ToUpperInvariant()))
                return (false, "Ya existe un paciente con esa cédula");
            if (dTO.fechaNacimiento.Date > DateTime.Today)
                return (false, "La fecha de nacimiento no puede ser futura");
            await repo.CreateAsync(new Paciente
            {
                Nombre = dTO.nombre.Trim(),
                Apellido = dTO.apellido.Trim(),
                Cedula = dTO.cedula.Trim().ToUpperInvariant(),
                FechaNacimiento = dTO.fechaNacimiento,
                Sexo = dTO.sexo.Trim(),
                Telefono = dTO.telefono.Trim(),
                Direccion = dTO.direccion.Trim()
            });
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id)
        {
            if (await repo.HasDependentsAsync(id))
                return (false, "No se puede eliminar el paciente porque tiene registros asociados");
            var deleted = await repo.DeleteAsync(id);
            return deleted ? (true, null) : (false, "Paciente no encontrado");
        }

        // Lista para llenar los combos de otros formularios
        public async Task<IReadOnlyList<PacienteTableDTO>> GetAllAsync()
        {
            var lista = await repo.GetAllAsync();
            return lista.Select(c => new PacienteTableDTO
            {
                id = c.Id,
                nombre = c.Nombre,
                apellido = c.Apellido,
                cedula = c.Cedula,
                fechaNacimiento = c.FechaNacimiento,
                sexo = c.Sexo,
                telefono = c.Telefono,
                direccion = c.Direccion,
                display = c.Nombre + " " + c.Apellido
            }).ToList();
        }

        public async Task<SavePacienteDTO?> GetByidAsync(int id)
        {
            var paciente = await repo.GetByIdAsync(id);
            if (paciente == null)
                return null;
            return new SavePacienteDTO
            {
                Id = paciente.Id,
                nombre = paciente.Nombre,
                apellido = paciente.Apellido,
                cedula = paciente.Cedula,
                fechaNacimiento = paciente.FechaNacimiento,
                sexo = paciente.Sexo,
                telefono = paciente.Telefono,
                direccion = paciente.Direccion
            };
        }

        public Task<datatableResponseDTO<PacienteTableDTO>> GetPagedAsync(DataTableRequestDTO request)
            => repo.GetPagedAsync(request);

        public async Task<(bool Success, string? ErrorMessage)> UpDateAsync(SavePacienteDTO dTO)
        {
            if (!dTO.Id.HasValue)
                return (false, "id invalido");
            var paciente = await repo.GetByIdAsync(dTO.Id.Value);
            if (paciente == null)
                return (false, "Paciente no encontrado");
            if (await repo.CedulaExisteAsync(dTO.cedula.Trim().ToUpperInvariant(), dTO.Id))
                return (false, "Ya existe un paciente con esa cédula");
            if (dTO.fechaNacimiento.Date > DateTime.Today)
                return (false, "La fecha de nacimiento no puede ser futura");
            paciente.Nombre = dTO.nombre.Trim();
            paciente.Apellido = dTO.apellido.Trim();
            paciente.Cedula = dTO.cedula.Trim().ToUpperInvariant();
            paciente.FechaNacimiento = dTO.fechaNacimiento;
            paciente.Sexo = dTO.sexo.Trim();
            paciente.Telefono = dTO.telefono.Trim();
            paciente.Direccion = dTO.direccion.Trim();
            await repo.UpdateAsync(paciente);
            return (true, null);
        }
    }
}
