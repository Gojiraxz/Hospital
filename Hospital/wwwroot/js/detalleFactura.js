$(function () {
    const config = window.detalleFacturaConfig || {};
    const modal = new bootstrap.Modal("#detalleFacturaModal");
    let isEdit = false;

    // Validaciones del formulario con JustValidate
    const detalleFacturaValidator = new JustValidate("#detalleFacturaForm", {
        validateBeforeSubmitting: true,
        focusInvalidField: true,
        errorFieldCssClass: 'is-invalid',
        successFieldCssClass: 'is-valid'
    });

    detalleFacturaValidator
        .addField("#detalleFacturaCantidad", [
            { rule: 'required', errorMessage: "El campo cantidad es obligatorio." },
            { rule: 'integer', errorMessage: "Debe ser un numero entero" },
            { rule: 'minNumber', value: 1, errorMessage: "Minimo permitido: 1" },
            { rule: 'maxNumber', value: 10000, errorMessage: "Maximo permitido: 10000" },
        ])
        .addField("#detalleFacturaPrecioUnitario", [
            { rule: 'required', errorMessage: "El campo precio unitario es obligatorio." },
            { rule: 'number', errorMessage: "Debe ser un numero" },
            { rule: 'minNumber', value: 0.01, errorMessage: "Minimo permitido: 0.01" },
        ])
        .addField("#detalleFacturaFacturaId", [
            { rule: 'required', errorMessage: "Seleccione factura." },
        ])
        .addField("#detalleFacturaMedicamentoId", [
            { rule: 'required', errorMessage: "Seleccione medicamento." },
        ]);

    // Tabla con paginacion, busqueda y orden en el servidor
    const table = $("#detalleFacturaTable").DataTable({
        processing: true,
        serverSide: true,
        ajax: {
            url: '/DetalleFactura/GetData',
            type: 'POST'
        },
        columns: [
            { data: 'id' },
            { data: 'cantidad' },
            { data: 'precioUnitario', render: d => money(d) },
            { data: 'subtotal', render: d => money(d) },
            { data: 'facturaName' },
            { data: 'medicamentoName' },
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
                        data-id="${data.id}" data-nombre="#${data.id}">
                        <i class="fa-solid fa-trash"></i></button>`;
                    }
                    return buttons || '<span class="text-muted">-</span>'
                }
            }
        ],
        language: {
            url: 'https://cdn.datatables.net/plug-ins/1.13.8/i18n/es-ES.json'
        },
        order: [[0, 'desc']]
    });

    function clearForm() {
        $("#detalleFacturaId").val('');
        $("#detalleFacturaCantidad").val('');
        $("#detalleFacturaPrecioUnitario").val('');
        $("#detalleFacturaFacturaId").val('');
        $("#detalleFacturaMedicamentoId").val('');
        detalleFacturaValidator.refresh();
    }

    $("#btnNuevoDetalleFactura").on('click', function () {
        isEdit = false;
        clearForm();
        $("#detalleFacturaModalTitle").text("Nuevo Detalle de factura");
        modal.show();
    });

    $('#detalleFacturaTable').on('click', '.btn-edit', async function () {
        const id = $(this).data('id');
        try {
            const response = await axios.post(`/DetalleFactura/GetByid?id=${id}`);
            isEdit = true;
            $('#detalleFacturaId').val(response.data.id);
            $('#detalleFacturaCantidad').val(response.data.cantidad);
            $('#detalleFacturaPrecioUnitario').val(response.data.precioUnitario);
            $('#detalleFacturaFacturaId').val(response.data.facturaId);
            $('#detalleFacturaMedicamentoId').val(response.data.medicamentoId);
            $('#detalleFacturaModalTitle').text("Editar Detalle de factura");
            detalleFacturaValidator.refresh();
            modal.show();

        } catch {
            Swal.fire('Error', 'No se pudo cargar el detalle de factura', 'error');
        }
    });
    $('#detalleFacturaTable').on('click', '.btn-delete', async function () {
        const id = $(this).data('id');
        const nombre = $(this).data('nombre')
        const confirm = await Swal.fire({
            title: '¿Eliminar detalle de factura?',
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
            const response = await axios.post(`/DetalleFactura/Delete?id=${id}`);
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

    detalleFacturaValidator.onSuccess(async (event) => {
        event.preventDefault();

        const payload = {
            id: $('#detalleFacturaId').val() ? parseInt($('#detalleFacturaId').val()) : null,
            cantidad: parseInt($('#detalleFacturaCantidad').val()),
            precioUnitario: parseFloat($('#detalleFacturaPrecioUnitario').val()),
            facturaId: parseInt($('#detalleFacturaFacturaId').val()),
            medicamentoId: parseInt($('#detalleFacturaMedicamentoId').val())
        };

        try {
            const response = isEdit
                ? await axios.post(`/DetalleFactura/Update`, payload)
                : await axios.post(`/DetalleFactura/Create`, payload);
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

    // Al elegir un medicamento se sugiere su precio
    $('#detalleFacturaMedicamentoId').on('change', function () {
        const precio = $(this).find(':selected').data('precio');
        if (precio !== undefined) $('#detalleFacturaPrecioUnitario').val(precio);
    });
});
