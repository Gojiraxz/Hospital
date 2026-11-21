$(function () {
    const config = window.habitacionConfig || {};
    const modal = new bootstrap.Modal("#habitacionModal");
    let isEdit = false;

    // Validaciones del formulario con JustValidate
    const habitacionValidator = new JustValidate("#habitacionForm", {
        validateBeforeSubmitting: true,
        focusInvalidField: true,
        errorFieldCssClass: 'is-invalid',
        successFieldCssClass: 'is-valid'
    });

    habitacionValidator
        .addField("#habitacionNumero", [
            { rule: 'required', errorMessage: "El campo número es obligatorio." },
            { rule: 'minLength', value: 1, errorMessage: "Minimo 1 caracteres" },
            { rule: 'maxLength', value: 45, errorMessage: "Maximo es de 45 caracteres" },
        ])
        .addField("#habitacionEstado", [
            { rule: 'required', errorMessage: "Seleccione estado." },
        ]);

    // Tabla con paginacion, busqueda y orden en el servidor
    const table = $("#habitacionTable").DataTable({
        processing: true,
        serverSide: true,
        ajax: {
            url: '/Habitacion/GetData',
            type: 'POST'
        },
        columns: [
            { data: 'id' },
            { data: 'numero' },
            { data: 'estado', render: d => badgeEstado(d) },
            { data: 'pacienteName', render: d => d || '<span class="text-muted">-</span>' },
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
                        data-id="${data.id}" data-nombre="${data.numero}">
                        <i class="fa-solid fa-trash"></i></button>`;
                    }
                    return buttons || '<span class="text-muted">-</span>'
                }
            }
        ],
        language: {
            url: 'https://cdn.datatables.net/plug-ins/1.13.8/i18n/es-ES.json'
        },
        order: [[1, 'asc']]
    });

    function clearForm() {
        $("#habitacionId").val('');
        $("#habitacionNumero").val('');
        $("#habitacionEstado").val('');
        $("#habitacionPacienteId").val('');
        habitacionValidator.refresh();
    }

    // Al elegir un paciente la habitacion pasa a Ocupada; sin paciente no puede quedar Ocupada
    $("#habitacionPacienteId").on('change', function () {
        if ($(this).val()) {
            $("#habitacionEstado").val('Ocupada');
        } else if ($("#habitacionEstado").val() === 'Ocupada') {
            $("#habitacionEstado").val('Disponible');
        }
    });
    $("#habitacionEstado").on('change', function () {
        if ($(this).val() !== 'Ocupada') {
            $("#habitacionPacienteId").val('');
        }
    });

    $("#btnNuevoHabitacion").on('click', function () {
        isEdit = false;
        clearForm();
        $("#habitacionModalTitle").text("Nueva Habitación");
        modal.show();
    });

    $('#habitacionTable').on('click', '.btn-edit', async function () {
        const id = $(this).data('id');
        try {
            const response = await axios.post(`/Habitacion/GetByid?id=${id}`);
            isEdit = true;
            $('#habitacionId').val(response.data.id);
            $('#habitacionNumero').val(response.data.numero);
            $('#habitacionEstado').val(response.data.estado);
            $('#habitacionPacienteId').val(response.data.pacienteId || '');
            $('#habitacionModalTitle').text("Editar Habitación");
            habitacionValidator.refresh();
            modal.show();

        } catch {
            Swal.fire('Error', 'No se pudo cargar la habitación', 'error');
        }
    });
    $('#habitacionTable').on('click', '.btn-delete', async function () {
        const id = $(this).data('id');
        const nombre = $(this).data('nombre')
        const confirm = await Swal.fire({
            title: '¿Eliminar habitación?',
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
            const response = await axios.post(`/Habitacion/Delete?id=${id}`);
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

    habitacionValidator.onSuccess(async (event) => {
        event.preventDefault();

        const payload = {
            id: $('#habitacionId').val() ? parseInt($('#habitacionId').val()) : null,
            numero: $('#habitacionNumero').val().trim(),
            estado: $('#habitacionEstado').val().trim(),
            pacienteId: $('#habitacionPacienteId').val() ? parseInt($('#habitacionPacienteId').val()) : null
        };

        try {
            const response = isEdit
                ? await axios.post(`/Habitacion/Update`, payload)
                : await axios.post(`/Habitacion/Create`, payload);
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
