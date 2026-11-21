$(function () {
    const config = window.examenMedicoConfig || {};
    const modal = new bootstrap.Modal("#examenMedicoModal");
    let isEdit = false;

    // Validaciones del formulario con JustValidate
    const examenMedicoValidator = new JustValidate("#examenMedicoForm", {
        validateBeforeSubmitting: true,
        focusInvalidField: true,
        errorFieldCssClass: 'is-invalid',
        successFieldCssClass: 'is-valid'
    });

    examenMedicoValidator
        .addField("#examenMedicoTipoExamen", [
            { rule: 'required', errorMessage: "El campo tipo de examen es obligatorio." },
            { rule: 'minLength', value: 2, errorMessage: "Minimo 2 caracteres" },
            { rule: 'maxLength', value: 45, errorMessage: "Maximo es de 45 caracteres" },
        ])
        .addField("#examenMedicoResultado", [
            { rule: 'required', errorMessage: "El campo resultado es obligatorio." },
            { rule: 'minLength', value: 2, errorMessage: "Minimo 2 caracteres" },
            { rule: 'maxLength', value: 250, errorMessage: "Maximo es de 250 caracteres" },
        ])
        .addField("#examenMedicoEstado", [
            { rule: 'required', errorMessage: "Seleccione estado." },
        ])
        .addField("#examenMedicoConsultaMedicaId", [
            { rule: 'required', errorMessage: "Seleccione consulta médica." },
        ])
        .addField("#examenMedicoLaboratorioId", [
            { rule: 'required', errorMessage: "Seleccione laboratorio." },
        ]);

    // Tabla con paginacion, busqueda y orden en el servidor
    const table = $("#examenMedicoTable").DataTable({
        processing: true,
        serverSide: true,
        ajax: {
            url: '/ExamenMedico/GetData',
            type: 'POST'
        },
        columns: [
            { data: 'id' },
            { data: 'tipoExamen' },
            { data: 'resultado' },
            { data: 'estado', render: d => badgeEstado(d) },
            { data: 'consultaMedicaName' },
            { data: 'laboratorioName' },
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
                        data-id="${data.id}" data-nombre="${data.tipoExamen}">
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
        $("#examenMedicoId").val('');
        $("#examenMedicoTipoExamen").val('');
        $("#examenMedicoResultado").val('');
        $("#examenMedicoEstado").val('');
        $("#examenMedicoConsultaMedicaId").val('');
        $("#examenMedicoLaboratorioId").val('');
        examenMedicoValidator.refresh();
    }

    $("#btnNuevoExamenMedico").on('click', function () {
        isEdit = false;
        clearForm();
        $("#examenMedicoModalTitle").text("Nuevo Examen médico");
        modal.show();
    });

    $('#examenMedicoTable').on('click', '.btn-edit', async function () {
        const id = $(this).data('id');
        try {
            const response = await axios.post(`/ExamenMedico/GetByid?id=${id}`);
            isEdit = true;
            $('#examenMedicoId').val(response.data.id);
            $('#examenMedicoTipoExamen').val(response.data.tipoExamen);
            $('#examenMedicoResultado').val(response.data.resultado);
            $('#examenMedicoEstado').val(response.data.estado);
            $('#examenMedicoConsultaMedicaId').val(response.data.consultaMedicaId);
            $('#examenMedicoLaboratorioId').val(response.data.laboratorioId);
            $('#examenMedicoModalTitle').text("Editar Examen médico");
            examenMedicoValidator.refresh();
            modal.show();

        } catch {
            Swal.fire('Error', 'No se pudo cargar el examen médico', 'error');
        }
    });
    $('#examenMedicoTable').on('click', '.btn-delete', async function () {
        const id = $(this).data('id');
        const nombre = $(this).data('nombre')
        const confirm = await Swal.fire({
            title: '¿Eliminar examen médico?',
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
            const response = await axios.post(`/ExamenMedico/Delete?id=${id}`);
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

    examenMedicoValidator.onSuccess(async (event) => {
        event.preventDefault();

        const payload = {
            id: $('#examenMedicoId').val() ? parseInt($('#examenMedicoId').val()) : null,
            tipoExamen: $('#examenMedicoTipoExamen').val().trim(),
            resultado: $('#examenMedicoResultado').val().trim(),
            estado: $('#examenMedicoEstado').val().trim(),
            consultaMedicaId: parseInt($('#examenMedicoConsultaMedicaId').val()),
            laboratorioId: parseInt($('#examenMedicoLaboratorioId').val())
        };

        try {
            const response = isEdit
                ? await axios.post(`/ExamenMedico/Update`, payload)
                : await axios.post(`/ExamenMedico/Create`, payload);
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
