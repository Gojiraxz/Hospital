$(function () {
    const config = window.consultaMedicaConfig || {};
    const modal = new bootstrap.Modal("#consultaMedicaModal");
    let isEdit = false;

    // Validaciones del formulario con JustValidate
    const consultaMedicaValidator = new JustValidate("#consultaMedicaForm", {
        validateBeforeSubmitting: true,
        focusInvalidField: true,
        errorFieldCssClass: 'is-invalid',
        successFieldCssClass: 'is-valid'
    });

    consultaMedicaValidator
        .addField("#consultaMedicaFecha", [
            { rule: 'required', errorMessage: "El campo fecha y hora es obligatorio." },
        ])
        .addField("#consultaMedicaDiagnostico", [
            { rule: 'required', errorMessage: "El campo diagnóstico es obligatorio." },
            { rule: 'minLength', value: 2, errorMessage: "Minimo 2 caracteres" },
            { rule: 'maxLength', value: 250, errorMessage: "Maximo es de 250 caracteres" },
        ])
        .addField("#consultaMedicaTratamiento", [
            { rule: 'required', errorMessage: "El campo tratamiento es obligatorio." },
            { rule: 'minLength', value: 2, errorMessage: "Minimo 2 caracteres" },
            { rule: 'maxLength', value: 250, errorMessage: "Maximo es de 250 caracteres" },
        ])
        .addField("#consultaMedicaDoctorId", [
            { rule: 'required', errorMessage: "Seleccione doctor." },
        ])
        .addField("#consultaMedicaCitaId", [
            { rule: 'required', errorMessage: "Seleccione cita." },
        ])
        .addField("#consultaMedicaPacienteId", [
            { rule: 'required', errorMessage: "Seleccione paciente." },
        ]);

    // Tabla con paginacion, busqueda y orden en el servidor
    const table = $("#consultaMedicaTable").DataTable({
        processing: true,
        serverSide: true,
        ajax: {
            url: '/ConsultaMedica/GetData',
            type: 'POST'
        },
        columns: [
            { data: 'id' },
            { data: 'fecha', render: d => fmtDateTime(d) },
            { data: 'diagnostico' },
            { data: 'tratamiento' },
            { data: 'doctorName' },
            { data: 'citaName' },
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
                        data-id="${data.id}" data-nombre="${data.pacienteName}">
                        <i class="fa-solid fa-trash"></i></button>`;
                    }
                    return buttons || '<span class="text-muted">-</span>'
                }
            }
        ],
        language: {
            url: 'https://cdn.datatables.net/plug-ins/1.13.8/i18n/es-ES.json'
        },
        order: [[1, 'desc']]
    });

    function clearForm() {
        $("#consultaMedicaId").val('');
        $("#consultaMedicaFecha").val('');
        $("#consultaMedicaDiagnostico").val('');
        $("#consultaMedicaTratamiento").val('');
        $("#consultaMedicaDoctorId").val('');
        $("#consultaMedicaCitaId").val('');
        $("#consultaMedicaPacienteId").val('');
        consultaMedicaValidator.refresh();
    }

    $("#btnNuevoConsultaMedica").on('click', function () {
        isEdit = false;
        clearForm();
        $("#consultaMedicaModalTitle").text("Nueva Consulta médica");
        modal.show();
    });

    $('#consultaMedicaTable').on('click', '.btn-edit', async function () {
        const id = $(this).data('id');
        try {
            const response = await axios.post(`/ConsultaMedica/GetByid?id=${id}`);
            isEdit = true;
            $('#consultaMedicaId').val(response.data.id);
            $('#consultaMedicaFecha').val((response.data.fecha || '').substring(0, 16));
            $('#consultaMedicaDiagnostico').val(response.data.diagnostico);
            $('#consultaMedicaTratamiento').val(response.data.tratamiento);
            $('#consultaMedicaDoctorId').val(response.data.doctorId);
            $('#consultaMedicaCitaId').val(response.data.citaId);
            $('#consultaMedicaPacienteId').val(response.data.pacienteId);
            $('#consultaMedicaModalTitle').text("Editar Consulta médica");
            consultaMedicaValidator.refresh();
            modal.show();

        } catch {
            Swal.fire('Error', 'No se pudo cargar la consulta médica', 'error');
        }
    });
    $('#consultaMedicaTable').on('click', '.btn-delete', async function () {
        const id = $(this).data('id');
        const nombre = $(this).data('nombre')
        const confirm = await Swal.fire({
            title: '¿Eliminar consulta médica?',
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
            const response = await axios.post(`/ConsultaMedica/Delete?id=${id}`);
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

    consultaMedicaValidator.onSuccess(async (event) => {
        event.preventDefault();

        const payload = {
            id: $('#consultaMedicaId').val() ? parseInt($('#consultaMedicaId').val()) : null,
            fecha: $('#consultaMedicaFecha').val(),
            diagnostico: $('#consultaMedicaDiagnostico').val().trim(),
            tratamiento: $('#consultaMedicaTratamiento').val().trim(),
            doctorId: parseInt($('#consultaMedicaDoctorId').val()),
            citaId: parseInt($('#consultaMedicaCitaId').val()),
            pacienteId: parseInt($('#consultaMedicaPacienteId').val())
        };

        try {
            const response = isEdit
                ? await axios.post(`/ConsultaMedica/Update`, payload)
                : await axios.post(`/ConsultaMedica/Create`, payload);
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
