$(function () {
    const config = window.pacienteConfig || {};
    const modal = new bootstrap.Modal("#pacienteModal");
    let isEdit = false;

    // Validaciones del formulario con JustValidate
    const pacienteValidator = new JustValidate("#pacienteForm", {
        validateBeforeSubmitting: true,
        focusInvalidField: true,
        errorFieldCssClass: 'is-invalid',
        successFieldCssClass: 'is-valid'
    });

    // Filtra mientras se escribe: cédula (14, sin guiones) y teléfono (8 dígitos)
    $(document).on('input', '#pacienteCedula', function () {
        this.value = this.value.replace(/[^0-9A-Za-z]/g, '').toUpperCase().slice(0, 14);
    });
    $(document).on('input', '#pacienteTelefono', function () {
        this.value = this.value.replace(/\D/g, '').slice(0, 8);
    });

    pacienteValidator
        .addField("#pacienteNombre", [
            { rule: 'required', errorMessage: "El campo nombre es obligatorio." },
            { rule: 'minLength', value: 2, errorMessage: "Minimo 2 caracteres" },
            { rule: 'maxLength', value: 45, errorMessage: "Maximo es de 45 caracteres" },
        ])
        .addField("#pacienteApellido", [
            { rule: 'required', errorMessage: "El campo apellido es obligatorio." },
            { rule: 'minLength', value: 2, errorMessage: "Minimo 2 caracteres" },
            { rule: 'maxLength', value: 45, errorMessage: "Maximo es de 45 caracteres" },
        ])
        .addField("#pacienteCedula", [
            { rule: 'required', errorMessage: "El campo cédula es obligatorio." },
            { rule: 'customRegexp', value: /^[0-9A-Za-z]{14}$/, errorMessage: "La cédula debe tener exactamente 14 caracteres (sin guiones)." },
        ])
        .addField("#pacienteFechaNacimiento", [
            { rule: 'required', errorMessage: "El campo fecha de nacimiento es obligatorio." },
        ])
        .addField("#pacienteSexo", [
            { rule: 'required', errorMessage: "Seleccione sexo." },
        ])
        .addField("#pacienteTelefono", [
            { rule: 'required', errorMessage: "El campo teléfono es obligatorio." },
            { rule: 'customRegexp', value: /^[0-9]{8}$/, errorMessage: "El teléfono debe tener exactamente 8 dígitos." },
        ])
        .addField("#pacienteDireccion", [
            { rule: 'required', errorMessage: "El campo dirección es obligatorio." },
            { rule: 'minLength', value: 2, errorMessage: "Minimo 2 caracteres" },
            { rule: 'maxLength', value: 150, errorMessage: "Maximo es de 150 caracteres" },
        ]);

    // Tabla con paginacion, busqueda y orden en el servidor
    const table = $("#pacienteTable").DataTable({
        processing: true,
        serverSide: true,
        ajax: {
            url: '/Paciente/GetData',
            type: 'POST'
        },
        columns: [
            { data: 'id' },
            { data: 'nombre' },
            { data: 'apellido' },
            { data: 'cedula' },
            { data: 'fechaNacimiento', render: d => fmtDate(d) },
            { data: 'sexo' },
            { data: 'telefono' },
            { data: 'direccion' },
            {
                data: null,
                orderable: false,
                searchable: false,
                render: function (data) {
                    let buttons = '';
                    if (config.canUpdate) {
                        buttons += `<button class="btn btn-sm btn-outline-primary me-1 btn-edit" data-id="${data.id}">
                        <i class="fa-solid fa-pen"></i></button>`;
                    }
                    if (config.canDelete) {
                        buttons += `<button class="btn btn-sm btn-outline-danger btn-delete" 
                        data-id="${data.id}" data-nombre="${data.nombre} ${data.apellido}">
                        <i class="fa-solid fa-trash"></i></button>`;
                    }
                    return buttons || '<span class="text-muted">-</span>'
                }
            }
        ],
        language: {
            url: 'https://cdn.datatables.net/plug-ins/1.13.8/i18n/es-ES.json'
        },
        order: [[2, 'asc']]
    });

    function clearForm() {
        $("#pacienteId").val('');
        $("#pacienteNombre").val('');
        $("#pacienteApellido").val('');
        $("#pacienteCedula").val('');
        $("#pacienteFechaNacimiento").val('');
        $("#pacienteSexo").val('');
        $("#pacienteTelefono").val('');
        $("#pacienteDireccion").val('');
        pacienteValidator.refresh();
    }

    $("#btnNuevoPaciente").on('click', function () {
        isEdit = false;
        clearForm();
        $("#pacienteModalTitle").text("Nuevo Paciente");
        modal.show();
    });

    $('#pacienteTable').on('click', '.btn-edit', async function () {
        const id = $(this).data('id');
        try {
            const response = await axios.post(`/Paciente/GetByid?id=${id}`);
            isEdit = true;
            $('#pacienteId').val(response.data.id);
            $('#pacienteNombre').val(response.data.nombre);
            $('#pacienteApellido').val(response.data.apellido);
            $('#pacienteCedula').val(response.data.cedula);
            $('#pacienteFechaNacimiento').val((response.data.fechaNacimiento || '').substring(0, 10));
            $('#pacienteSexo').val(response.data.sexo);
            $('#pacienteTelefono').val(response.data.telefono);
            $('#pacienteDireccion').val(response.data.direccion);
            $('#pacienteModalTitle').text("Editar Paciente");
            pacienteValidator.refresh();
            modal.show();

        } catch {
            Swal.fire('Error', 'No se pudo cargar el paciente', 'error');
        }
    });
    $('#pacienteTable').on('click', '.btn-delete', async function () {
        const id = $(this).data('id');
        const nombre = $(this).data('nombre')
        const confirm = await Swal.fire({
            title: '¿Eliminar paciente?',
            html: `¿Seguro que deseas eliminar <strong>${nombre}</strong>?`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Si, eliminar',
            cancelButtonText: 'Cancelar',
            confirmButtonColor: '#d33',
            cancelButtonColor: '#6c757d'
        });
        if (!confirm.isConfirmed) return
        try {
            const response = await axios.post(`/Paciente/Delete?id=${id}`);
            if (response.data.success) {
                table.ajax.reload();
                Swal.fire('Eliminado', response.data.message, 'success');
            }
        }
        catch (error) {
            const message = error.response?.data?.message || 'No se pudo eliminar';
            Swal.fire('Error', message, 'error');
        }
    });

    pacienteValidator.onSuccess(async (event) => {
        event.preventDefault();

        const payload = {
            id: $('#pacienteId').val() ? parseInt($('#pacienteId').val()) : null,
            nombre: $('#pacienteNombre').val().trim(),
            apellido: $('#pacienteApellido').val().trim(),
            cedula: $('#pacienteCedula').val().trim(),
            fechaNacimiento: $('#pacienteFechaNacimiento').val(),
            sexo: $('#pacienteSexo').val().trim(),
            telefono: $('#pacienteTelefono').val().trim(),
            direccion: $('#pacienteDireccion').val().trim()
        };

        try {
            const response = isEdit
                ? await axios.post(`/Paciente/Update`, payload)
                : await axios.post(`/Paciente/Create`, payload);
            if (response.data.success) {
                modal.hide();
                table.ajax.reload();
                Swal.fire({
                    title: 'Listo',
                    text: response.data.message,
                    icon: 'success',
                    timer: 2000,
                    showConfirmButton: false
                });
            }

        } catch (error) {
            const message = error.response?.data?.message || 'No se pudo guardar.';
            Swal.fire('Error', message, 'error');
        }
    })
});
