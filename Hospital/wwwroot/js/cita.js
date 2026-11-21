$(function () {
    const config = window.citaConfig || {};
    const modal = new bootstrap.Modal("#citaModal");
    let isEdit = false;

    // Validaciones del formulario con JustValidate
    const citaValidator = new JustValidate("#citaForm", {
        validateBeforeSubmitting: true,
        focusInvalidField: true,
        errorFieldCssClass: 'is-invalid',
        successFieldCssClass: 'is-valid'
    });

    citaValidator
        .addField("#citaFecha", [
            { rule: 'required', errorMessage: "El campo fecha y hora es obligatorio." },
        ])
        .addField("#citaEstado", [
            { rule: 'required', errorMessage: "Seleccione estado." },
        ])
        .addField("#citaMotivo", [
            { rule: 'required', errorMessage: "El campo motivo es obligatorio." },
            { rule: 'minLength', value: 2, errorMessage: "Minimo 2 caracteres" },
            { rule: 'maxLength', value: 250, errorMessage: "Maximo es de 250 caracteres" },
        ])
        .addField("#citaDoctorId", [
            { rule: 'required', errorMessage: "Seleccione doctor." },
        ])
        .addField("#citaPacienteId", [
            { rule: 'required', errorMessage: "Seleccione paciente." },
        ]);

    // Tabla con paginacion, busqueda y orden en el servidor
    const table = $("#citaTable").DataTable({
        processing: true,
        serverSide: true,
        ajax: {
            url: '/Cita/GetData',
            type: 'POST'
        },
        columns: [
            { data: 'id' },
            { data: 'fecha', render: d => fmtDateTime(d) },
            { data: 'estado', render: d => badgeEstado(d) },
            { data: 'motivo' },
            { data: 'doctorName' },
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
        $("#citaId").val('');
        $("#citaFecha").val('');
        $("#citaEstado").val('');
        $("#citaMotivo").val('');
        $("#citaDoctorId").val('');
        $("#citaPacienteId").val('');
        citaValidator.refresh();
    }

    $("#btnNuevoCita").on('click', function () {
        isEdit = false;
        clearForm();
        $("#citaModalTitle").text("Nueva Cita");
        modal.show();
    });

    $('#citaTable').on('click', '.btn-edit', async function () {
        const id = $(this).data('id');
        try {
            const response = await axios.post(`/Cita/GetByid?id=${id}`);
            isEdit = true;
            $('#citaId').val(response.data.id);
            $('#citaFecha').val((response.data.fecha || '').substring(0, 16));
            $('#citaEstado').val(response.data.estado);
            $('#citaMotivo').val(response.data.motivo);
            $('#citaDoctorId').val(response.data.doctorId);
            $('#citaPacienteId').val(response.data.pacienteId);
            $('#citaModalTitle').text("Editar Cita");
            citaValidator.refresh();
            modal.show();

        } catch {
            Swal.fire('Error', 'No se pudo cargar la cita', 'error');
        }
    });
    $('#citaTable').on('click', '.btn-delete', async function () {
        const id = $(this).data('id');
        const nombre = $(this).data('nombre')
        const confirm = await Swal.fire({
            title: '¿Eliminar cita?',
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
            const response = await axios.post(`/Cita/Delete?id=${id}`);
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

    citaValidator.onSuccess(async (event) => {
        event.preventDefault();

        const payload = {
            id: $('#citaId').val() ? parseInt($('#citaId').val()) : null,
            fecha: $('#citaFecha').val(),
            estado: $('#citaEstado').val().trim(),
            motivo: $('#citaMotivo').val().trim(),
            doctorId: parseInt($('#citaDoctorId').val()),
            pacienteId: parseInt($('#citaPacienteId').val())
        };

        try {
            const response = isEdit
                ? await axios.post(`/Cita/Update`, payload)
                : await axios.post(`/Cita/Create`, payload);
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
