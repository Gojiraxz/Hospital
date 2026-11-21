document.addEventListener('DOMContentLoaded', () => {

    // ===================== Utilidades =====================
    const forms = document.querySelectorAll('.form');

    const showForm = (formId) => {
        forms.forEach(f => f.classList.remove('active'));
        document.getElementById(formId)?.classList.add('active');
    };

    document.querySelectorAll('.switch-form, .forgot-link').forEach(link => {
        link.addEventListener('click', (e) => {
            e.preventDefault();
            showForm(link.dataset.form);
        });
    });

    const toast = (icon, title, timer = 3000) => Swal.fire({
        toast: true,
        position: 'top-end',
        icon,
        title,
        showConfirmButton: false,
        timer
    });

    // Distingue "el servidor respondio con un error" de "no hay conexion"
    const mensajeError = (err, porDefecto) => {
        if (err.response) {
            return err.response.data?.errorMessage
                || err.response.data?.message
                || `${porDefecto} (el servidor respondió con el error ${err.response.status}).`;
        }
        return 'No se pudo conectar con el servidor. Verifica que la aplicación esté en ejecución.';
    };

    const errorPopup = (err, porDefecto) =>
        Swal.fire({ icon: 'error', title: 'Error', text: mensajeError(err, porDefecto) });

    // ===================== Crear cuenta de administrador (solo primera vez) =====================
    if (document.getElementById('setupFormElement')) {
        const validateSetup = new JustValidate('#setupFormElement');

        validateSetup
            .addField('#setupUser', [
                { rule: 'required', errorMessage: 'El nombre de usuario es requerido.' },
                { rule: 'minLength', value: 3, errorMessage: 'Mínimo 3 caracteres.' }
            ])
            .addField('#setupEmail', [
                { rule: 'required', errorMessage: 'El correo es requerido.' },
                { rule: 'email', errorMessage: 'Ingresa un correo válido.' }
            ])
            .addField('#setupPassword', [
                { rule: 'required', errorMessage: 'La contraseña es requerida.' },
                { rule: 'minLength', value: 8, errorMessage: 'Mínimo 8 caracteres.' }
            ])
            .addField('#setupConfirm', [
                { rule: 'required', errorMessage: 'Confirma la contraseña.' },
                {
                    validator: (value, fields) => value === fields['#setupPassword'].elem.value,
                    errorMessage: 'Las contraseñas no coinciden.'
                }
            ])
            .onSuccess(async (event) => {
                event.preventDefault();
                const data = {
                    username: document.getElementById('setupUser').value.trim(),
                    email: document.getElementById('setupEmail').value.trim(),
                    password: document.getElementById('setupPassword').value
                };
                try {
                    const response = await axios.post('/Auth/SetupAdmin', data);
                    if (response.data.success) {
                        await Swal.fire({
                            icon: 'success',
                            title: 'Cuenta creada',
                            text: 'Ahora inicia sesión con tu nombre de usuario y contraseña.'
                        });
                        window.location.href = '/Auth/Login';
                    }
                } catch (err) {
                    errorPopup(err, 'No se pudo crear la cuenta');
                }
            });
    }

    // ===================== Iniciar sesion (admin / doctor) =====================
    if (document.getElementById('signupForm')) {
        const validateLogin = new JustValidate('#signupForm');

        validateLogin
            .addField('#username', [
                { rule: 'required', errorMessage: 'El nombre de usuario es requerido.' }
            ])
            .addField('#password', [
                { rule: 'required', errorMessage: 'La contraseña es requerida.' }
            ])
            .onSuccess(async (event) => {
                event.preventDefault();
                const data = {
                    username: document.getElementById('username').value.trim(),
                    password: document.getElementById('password').value
                };
                try {
                    const response = await axios.post('/Auth/Login', data);
                    if (response.data.success) {
                        await toast('success', response.data.message || 'Login exitoso!', 1500);
                        window.location.href = response.data.redirect || '/Home/Index';
                    } else {
                        Swal.fire({
                            icon: 'error',
                            title: 'Error',
                            text: response.data.errorMessage || 'No se pudo iniciar sesion.'
                        });
                    }
                } catch (err) {
                    errorPopup(err, 'No se pudo iniciar sesión');
                }
            });
    }

    // ===================== Acceso de pacientes (cedula) =====================
    if (document.getElementById('pacienteFormElement')) {
        const validatePaciente = new JustValidate('#pacienteFormElement');

        validatePaciente
            .addField('#cedula', [
                { rule: 'required', errorMessage: 'Ingresa tu número de cédula.' },
                { rule: 'minLength', value: 5, errorMessage: 'La cédula es muy corta.' }
            ])
            .onSuccess(async (event) => {
                event.preventDefault();
                const data = { cedula: document.getElementById('cedula').value.trim() };
                try {
                    const response = await axios.post('/Auth/PacienteLogin', data);
                    if (response.data.success) {
                        await toast('success', response.data.message || 'Bienvenido', 1200);
                        window.location.href = response.data.redirect || '/Portal';
                    }
                } catch (err) {
                    errorPopup(err, 'No se pudo iniciar sesión');
                }
            });
    }

    // ===================== Olvide mi contraseña =====================
    if (document.getElementById('resetPasswordForm')) {
        const validateForgot = new JustValidate('#resetPasswordForm');

        validateForgot
            .addField('#resetEmail', [
                { rule: 'required', errorMessage: 'El correo es requerido.' },
                { rule: 'email', errorMessage: 'Ingresa un correo válido.' }
            ])
            .onSuccess(async (event) => {
                event.preventDefault();

                // ForgetPassword no usa [FromBody], por eso se envia como formulario
                const data = new URLSearchParams();
                data.append('email', document.getElementById('resetEmail').value);

                try {
                    const response = await axios.post('/Auth/ForgetPassword', data);

                    if (response.data.success) {
                        await Swal.fire({
                            icon: 'success',
                            title: 'Correo enviado',
                            text: 'Te enviamos un enlace para restablecer tu contraseña.'
                        });
                        document.getElementById('resetPasswordForm').reset();
                        showForm('loginForm');
                    }
                } catch (err) {
                    errorPopup(err, 'No se pudo enviar el correo');
                }
            });
    }
});
