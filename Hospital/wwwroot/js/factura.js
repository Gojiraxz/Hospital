$(function () {
    const config = window.facturaConfig || {};
    const modal = new bootstrap.Modal("#facturaModal");
    let isEdit = false;

    // Validaciones del formulario con JustValidate
    const facturaValidator = new JustValidate("#facturaForm", {
        validateBeforeSubmitting: true,
        focusInvalidField: true,
        errorFieldCssClass: 'is-invalid',
        successFieldCssClass: 'is-valid'
    });

    facturaValidator
        .addField("#facturaFecha", [
            { rule: 'required', errorMessage: "El campo fecha es obligatorio." },
        ])
        .addField("#facturaEstado", [
            { rule: 'required', errorMessage: "Seleccione estado." },
        ])
        .addField("#facturaPacienteId", [
            { rule: 'required', errorMessage: "Seleccione paciente." },
        ]);

    // Tabla con paginacion, busqueda y orden en el servidor
    const table = $("#facturaTable").DataTable({
        processing: true,
        serverSide: true,
        ajax: {
            url: '/Factura/GetData',
            type: 'POST'
        },
        columns: [
            { data: 'id' },
            { data: 'fecha', render: d => fmtDate(d) },
            { data: 'total', render: d => money(d) },
            { data: 'estado', render: d => badgeEstado(d) },
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
        $("#facturaId").val('');
        $("#facturaFecha").val('');
        $("#facturaEstado").val('');
        $("#facturaPacienteId").val('');
        facturaValidator.refresh();
    }

    $("#btnNuevoFactura").on('click', function () {
        isEdit = false;
        clearForm();
        $("#facturaModalTitle").text("Nueva Factura");
        modal.show();
    });

    $('#facturaTable').on('click', '.btn-edit', async function () {
        const id = $(this).data('id');
        try {
            const response = await axios.post(`/Factura/GetByid?id=${id}`);
            isEdit = true;
            $('#facturaId').val(response.data.id);
            $('#facturaFecha').val((response.data.fecha || '').substring(0, 10));
            $('#facturaEstado').val(response.data.estado);
            $('#facturaPacienteId').val(response.data.pacienteId);
            $('#facturaModalTitle').text("Editar Factura");
            facturaValidator.refresh();
            modal.show();

        } catch {
            Swal.fire('Error', 'No se pudo cargar la factura', 'error');
        }
    });
    $('#facturaTable').on('click', '.btn-delete', async function () {
        const id = $(this).data('id');
        const nombre = $(this).data('nombre')
        const confirm = await Swal.fire({
            title: '¿Eliminar factura?',
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
            const response = await axios.post(`/Factura/Delete?id=${id}`);
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

    facturaValidator.onSuccess(async (event) => {
        event.preventDefault();

        const payload = {
            id: $('#facturaId').val() ? parseInt($('#facturaId').val()) : null,
            fecha: $('#facturaFecha').val(),
            estado: $('#facturaEstado').val().trim(),
            pacienteId: parseInt($('#facturaPacienteId').val())
        };

        try {
            const response = isEdit
                ? await axios.post(`/Factura/Update`, payload)
                : await axios.post(`/Factura/Create`, payload);
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
