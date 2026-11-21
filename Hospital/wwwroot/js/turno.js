$(function () {
    const config = window.turnoConfig || {};
    const modal = new bootstrap.Modal("#turnoModal");
    let isEdit = false;

    // Validaciones del formulario con JustValidate
    const turnoValidator = new JustValidate("#turnoForm", {
        validateBeforeSubmitting: true,
        focusInvalidField: true,
        errorFieldCssClass: 'is-invalid',
        successFieldCssClass: 'is-valid'
    });

    turnoValidator
        .addField("#turnoInicio", [
            { rule: 'required', errorMessage: "El campo hora de inicio es obligatorio." },
        ])
        .addField("#turnoFinal", [
            { rule: 'required', errorMessage: "El campo hora final es obligatorio." },
        ])
        .addField("#turnoEnfermeroId", [
            { rule: 'required', errorMessage: "Seleccione enfermero." },
        ])
        .addField("#turnoDoctorId", [
            { rule: 'required', errorMessage: "Seleccione doctor." },
        ]);

    // Tabla con paginacion, busqueda y orden en el servidor
    const table = $("#turnoTable").DataTable({
        processing: true,
        serverSide: true,
        ajax: {
            url: '/Turno/GetData',
            type: 'POST'
        },
        columns: [
            { data: 'id' },
            { data: 'inicio' },
            { data: 'final' },
            { data: 'enfermeroName' },
            { data: 'doctorName' },
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
                        data-id="${data.id}" data-nombre="${data.enfermeroName}">
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
        $("#turnoId").val('');
        $("#turnoInicio").val('');
        $("#turnoFinal").val('');
        $("#turnoEnfermeroId").val('');
        $("#turnoDoctorId").val('');
        turnoValidator.refresh();
    }

    $("#btnNuevoTurno").on('click', function () {
        isEdit = false;
        clearForm();
        $("#turnoModalTitle").text("Nuevo Turno");
        modal.show();
    });

    $('#turnoTable').on('click', '.btn-edit', async function () {
        const id = $(this).data('id');
        try {
            const response = await axios.post(`/Turno/GetByid?id=${id}`);
            isEdit = true;
            $('#turnoId').val(response.data.id);
            $('#turnoInicio').val(response.data.inicio);
            $('#turnoFinal').val(response.data.final);
            $('#turnoEnfermeroId').val(response.data.enfermeroId);
            $('#turnoDoctorId').val(response.data.doctorId);
            $('#turnoModalTitle').text("Editar Turno");
            turnoValidator.refresh();
            modal.show();

        } catch {
            Swal.fire('Error', 'No se pudo cargar el turno', 'error');
        }
    });
    $('#turnoTable').on('click', '.btn-delete', async function () {
        const id = $(this).data('id');
        const nombre = $(this).data('nombre')
        const confirm = await Swal.fire({
            title: '¿Eliminar turno?',
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
            const response = await axios.post(`/Turno/Delete?id=${id}`);
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

    turnoValidator.onSuccess(async (event) => {
        event.preventDefault();

        const payload = {
            id: $('#turnoId').val() ? parseInt($('#turnoId').val()) : null,
            inicio: $('#turnoInicio').val().trim(),
            final: $('#turnoFinal').val().trim(),
            enfermeroId: parseInt($('#turnoEnfermeroId').val()),
            doctorId: parseInt($('#turnoDoctorId').val())
        };

        try {
            const response = isEdit
                ? await axios.post(`/Turno/Update`, payload)
                : await axios.post(`/Turno/Create`, payload);
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
