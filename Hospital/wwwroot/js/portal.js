document.addEventListener('DOMContentLoaded', () => {
    const citaModal = new bootstrap.Modal('#citaModal');
    const recetaModal = new bootstrap.Modal('#recetaModal');
    const $ = (id) => document.getElementById(id);

    // La fecha de la cita no puede estar en el pasado
    const fechaMinima = () => {
        const now = new Date();
        now.setMinutes(now.getMinutes() - now.getTimezoneOffset());
        return now.toISOString().slice(0, 16);
    };

    const mensajeError = (err, porDefecto) => {
        if (err.response) {
            return err.response.data?.message || err.response.data?.errorMessage
                || `${porDefecto} (error ${err.response.status}).`;
        }
        return 'No se pudo conectar con el servidor.';
    };

    const aviso = (texto) => Swal.fire({ icon: 'warning', title: 'Falta información', text: texto });

    $('btnNuevaCita')?.addEventListener('click', () => {
        $('citaForm').reset();
        $('citaFecha').min = fechaMinima();
        citaModal.show();
    });

    $('btnNuevaReceta')?.addEventListener('click', () => {
        $('recetaForm').reset();
        recetaModal.show();
    });

    // ---- Pedir cita
    $('citaForm').addEventListener('submit', async (e) => {
        e.preventDefault();
        const doctorId = parseInt($('citaDoctorId').value);
        const fecha = $('citaFecha').value;
        const motivo = $('citaMotivo').value.trim();
        if (!doctorId) return aviso('Selecciona un doctor.');
        if (!fecha) return aviso('Selecciona la fecha y hora.');
        if (motivo.length < 2) return aviso('Escribe el motivo de la consulta.');

        try {
            const response = await axios.post('/Portal/SolicitarCita', { doctorId, fecha, motivo });
            citaModal.hide();
            await Swal.fire({ icon: 'success', title: 'Listo', text: response.data.message, timer: 2200, showConfirmButton: false });
            location.reload();
        } catch (err) {
            Swal.fire('Error', mensajeError(err, 'No se pudo solicitar la cita'), 'error');
        }
    });

    // ---- Pedir receta
    $('recetaForm').addEventListener('submit', async (e) => {
        e.preventDefault();
        const doctorId = parseInt($('recetaDoctorId').value);
        const medicamento = $('recetaMedicamento').value.trim();
        const indicaciones = $('recetaIndicaciones').value.trim();
        if (!doctorId) return aviso('Selecciona un doctor.');
        if (medicamento.length < 2) return aviso('Escribe el medicamento que necesitas.');
        if (indicaciones.length < 2) return aviso('Escribe las indicaciones o el motivo.');

        try {
            const response = await axios.post('/Portal/SolicitarReceta', { doctorId, medicamento, indicaciones });
            recetaModal.hide();
            await Swal.fire({ icon: 'success', title: 'Listo', text: response.data.message, timer: 2200, showConfirmButton: false });
            location.reload();
        } catch (err) {
            Swal.fire('Error', mensajeError(err, 'No se pudo solicitar la receta'), 'error');
        }
    });

    // ---- Cerrar sesion
    $('logoutBtn').addEventListener('click', async () => {
        try {
            await axios.post('/Auth/Logout');
        } finally {
            window.location.href = '/Auth/Login';
        }
    });
});
