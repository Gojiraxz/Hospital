$(function () {
    const form = document.getElementById('formNuevoUsuario');
    if (!form) return;

    const modalEl = document.getElementById('modalNuevoUsuario');
    const btnGuardar = document.getElementById('btnGuardarUsuario');

    const error = (text) => Swal.fire({ icon: 'error', title: 'Error', text });

    form.addEventListener('submit', async (e) => {
        e.preventDefault();

        const username = document.getElementById('nuUsername').value.trim();
        const email = document.getElementById('nuEmail').value.trim();
        const password = document.getElementById('nuPassword').value;
        const confirm = document.getElementById('nuConfirm').value;
        const roleId = parseInt(document.getElementById('nuRole').value, 10);

        if (username.length < 3) return error('El nombre de usuario debe tener mínimo 3 caracteres.');
        if (!email || !document.getElementById('nuEmail').checkValidity()) return error('Ingresa un correo válido.');
        if (password.length < 8) return error('La contraseña debe tener mínimo 8 caracteres.');
        if (password !== confirm) return error('Las contraseñas no coinciden.');
        if (!roleId) return error('Selecciona un rol.');

        btnGuardar.disabled = true;
        try {
            const response = await axios.post('/Admin/CreateUser', { username, email, password, roleId });
            if (response.data.success) {
                bootstrap.Modal.getInstance(modalEl)?.hide();
                const result = await Swal.fire({
                    icon: 'success',
                    title: 'Usuario creado',
                    text: response.data.message,
                    showCancelButton: true,
                    confirmButtonText: 'Configurar permisos',
                    cancelButtonText: 'Cerrar'
                });
                if (result.isConfirmed && response.data.userId) {
                    window.location.href = '/Admin/UserPermission/' + response.data.userId;
                } else {
                    window.location.reload();
                }
            } else {
                error(response.data.message || 'No se pudo crear el usuario.');
            }
        } catch (err) {
            if (err.response) {
                error(err.response.data?.message || `El servidor respondió con el error ${err.response.status}.`);
            } else {
                error('No se pudo conectar con el servidor.');
            }
        } finally {
            btnGuardar.disabled = false;
        }
    });

    // Limpia el formulario cada vez que se cierra el modal
    modalEl.addEventListener('hidden.bs.modal', () => form.reset());
});
