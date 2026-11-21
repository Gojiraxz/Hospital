document.addEventListener('DOMContentLoaded', () => {

    const mostrarError = (mensaje) => Swal.fire({ icon: 'error', title: 'Error', text: mensaje });

    const llamar = async (url, params) => {
        try {
            const response = await axios.post(url, null, { params });
            const data = response.data;
            if (data && data.message && data.message !== 'Permiso asignado' && data.message !== 'Permiso eliminado') {
                // El controlador responde success=true aunque haya un mensaje de error de negocio
                await Swal.fire({ icon: 'info', title: 'Aviso', text: data.message });
            }
            location.reload();
        } catch (err) {
            mostrarError(err.response?.data?.message || 'No se pudo completar la operacion.');
        }
    };

    // ----- Asignar permiso a un rol -----
    const btnAsignar = document.getElementById('btnAssignPermission');
    btnAsignar?.addEventListener('click', () => {
        const permissionId = document.getElementById('permissionToAssign').value;
        llamar('/Admin/AssignPermissionToRole', {
            roleId: btnAsignar.dataset.roleId,
            permissionId
        });
    });

    // ----- Quitar permiso de un rol -----
    document.querySelectorAll('.btn-toggle-permission').forEach(btn => {
        btn.addEventListener('click', async () => {
            const confirmacion = await Swal.fire({
                icon: 'warning',
                title: 'Quitar permiso',
                text: 'Este rol perdera el permiso seleccionado.',
                showCancelButton: true,
                confirmButtonText: 'Si, quitar',
                cancelButtonText: 'Cancelar'
            });
            if (!confirmacion.isConfirmed) return;

            llamar('/Admin/RemovePermissionFromRole', {
                roleId: btn.dataset.roleId,
                permissionId: btn.dataset.permissionId
            });
        });
    });
});
