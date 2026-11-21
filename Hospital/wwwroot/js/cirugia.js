$(function () {
    const config = window.cirugiaConfig || {};
    const modal = new bootstrap.Modal("#cirugiaModal");
    let isEdit = false;

    // Validaciones del formulario con JustValidate
    const cirugiaValidator = new JustValidate("#cirugiaForm", {
        validateBeforeSubmitting: true,
        focusInvalidField: true,
        errorFieldCssClass: 'is-invalid',
        successFieldCssClass: 'is-valid'
    });

    cirugiaValidator
        .addField("#cirugiaNombre", [
            { rule: 'required', errorMessage: "El campo nombre es obligatorio." },
            { rule: 'minLength', value: 2, errorMessage: "Minimo 2 caracteres" },
            { rule: 'maxLength', value: 45, errorMessage: "Maximo es de 45 caracteres" },
        ])
        .addField("#cirugiaFecha", [
            { rule: 'required', errorMessage: "El campo fecha y hora es obligatorio." },
        ])
        .addField("#cirugiaEstado", [
            { rule: 'required', errorMessage: "Seleccione estado." },
        ])
        .addField("#cirugiaDoctorId", [
            { rule: 'required', errorMessage: "Seleccione doctor." },
        ])
        .addField("#cirugiaHabitacionId", [
            { rule: 'required', errorMessage: "Seleccione habitación." },
        ])
        .addField("#cirugiaPacienteId", [
            { rule: 'required', errorMessage: "Seleccione paciente." },
        ]);

    // Tabla con paginacion, busqueda y orden en el servidor
    const table = $("#cirugiaTable").DataTable({
        processing: true,
        serverSide: true,
        ajax: {
            url: '/Cirugia/GetData',
            type: 'POST'
        },
        columns: [
            { data: 'id' },
            { data: 'nombre' },
            { data: 'fecha', render: d => fmtDateTime(d) },
            { data: 'estado', render: d => badgeEstado(d) },
            { data: 'doctorName' },
            { data: 'habitacionName' },
            { data: 'pacienteName' },
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
                        data-id="${data.id}" data-nombre="${data.nombre}">
                        <i class="fa-solid fa-trash"></i></button>`;
                    }
                    return buttons || '<span class="text-muted">-</span>'
                }
            }
        ],
        language: {
            url: 'https://cdn.datatables.net/plug-ins/1.13.8/i18n/es-ES.json'
        },
        order: [[2, 'desc']]
    });

    function clearForm() {
        $("#cirugiaId").val('');
        $("#cirugiaNombre").val('');
        $("#cirugiaFecha").val('');
        $("#cirugiaEstado").val('');
        $("#cirugiaDoctorId").val('');
        $("#cirugiaHabitacionId").val('');
        $("#cirugiaPacienteId").val('');
        cirugiaValidator.refresh();
    }

    $("#btnNuevoCirugia").on('click', function () {
        isEdit = false;
        clearForm();
        $("#cirugiaModalTitle").text("Nueva Cirugía");
        modal.show();
    });

    $('#cirugiaTable').on('click', '.btn-edit', async function () {
        const id = $(this).data('id');
        try {
            const response = await axios.post(`/Cirugia/GetByid?id=${id}`);
            isEdit = true;
            $('#cirugiaId').val(response.data.id);
            $('#cirugiaNombre').val(response.data.nombre);
            $('#cirugiaFecha').val((response.data.fecha || '').substring(0, 16));
            $('#cirugiaEstado').val(response.data.estado);
            $('#cirugiaDoctorId').val(response.data.doctorId);
            $('#cirugiaHabitacionId').val(response.data.habitacionId);
            $('#cirugiaPacienteId').val(response.data.pacienteId);
            $('#cirugiaModalTitle').text("Editar Cirugía");
            cirugiaValidator.refresh();
            modal.show();

        } catch {
            Swal.fire('Error', 'No se pudo cargar la cirugía', 'error');
        }
    });
    $('#cirugiaTable').on('click', '.btn-delete', async function () {
        const id = $(this).data('id');
        const nombre = $(this).data('nombre')
        const confirm = await Swal.fire({
            title: '¿Eliminar cirugía?',
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
            const response = await axios.post(`/Cirugia/Delete?id=${id}`);
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

    cirugiaValidator.onSuccess(async (event) => {
        event.preventDefault();

        const payload = {
            id: $('#cirugiaId').val() ? parseInt($('#cirugiaId').val()) : null,
            nombre: $('#cirugiaNombre').val().trim(),
            fecha: $('#cirugiaFecha').val(),
            estado: $('#cirugiaEstado').val().trim(),
            doctorId: parseInt($('#cirugiaDoctorId').val()),
            habitacionId: parseInt($('#cirugiaHabitacionId').val()),
            pacienteId: parseInt($('#cirugiaPacienteId').val())
        };

        try {
            const response = isEdit
                ? await axios.post(`/Cirugia/Update`, payload)
                : await axios.post(`/Cirugia/Create`, payload);
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
