$(function () {
    const config = window.medicamentoConfig || {};
    const modal = new bootstrap.Modal("#medicamentoModal");
    let isEdit = false;

    // Validaciones del formulario con JustValidate
    const medicamentoValidator = new JustValidate("#medicamentoForm", {
        validateBeforeSubmitting: true,
        focusInvalidField: true,
        errorFieldCssClass: 'is-invalid',
        successFieldCssClass: 'is-valid'
    });

    medicamentoValidator
        .addField("#medicamentoNombre", [
            { rule: 'required', errorMessage: "El campo nombre es obligatorio." },
            { rule: 'minLength', value: 2, errorMessage: "Minimo 2 caracteres" },
            { rule: 'maxLength', value: 45, errorMessage: "Maximo es de 45 caracteres" },
        ])
        .addField("#medicamentoDescripcion", [
            { rule: 'required', errorMessage: "El campo descripción es obligatorio." },
            { rule: 'minLength', value: 2, errorMessage: "Minimo 2 caracteres" },
            { rule: 'maxLength', value: 250, errorMessage: "Maximo es de 250 caracteres" },
        ])
        .addField("#medicamentoStock", [
            { rule: 'required', errorMessage: "El campo stock es obligatorio." },
            { rule: 'integer', errorMessage: "Debe ser un numero entero" },
            { rule: 'minNumber', value: 0, errorMessage: "Minimo permitido: 0" },
            { rule: 'maxNumber', value: 100000, errorMessage: "Maximo permitido: 100000" },
        ])
        .addField("#medicamentoPrecio", [
            { rule: 'required', errorMessage: "El campo precio es obligatorio." },
            { rule: 'number', errorMessage: "Debe ser un numero" },
            { rule: 'minNumber', value: 0.01, errorMessage: "Minimo permitido: 0.01" },
        ])
        .addField("#medicamentoProveedorId", [
            { rule: 'required', errorMessage: "Seleccione proveedor." },
        ]);

    // Tabla con paginacion, busqueda y orden en el servidor
    const table = $("#medicamentoTable").DataTable({
        processing: true,
        serverSide: true,
        ajax: {
            url: '/Medicamento/GetData',
            type: 'POST'
        },
        columns: [
            { data: 'id' },
            { data: 'nombre' },
            { data: 'descripcion' },
            { data: 'stock' },
            { data: 'precio', render: d => money(d) },
            { data: 'proveedorName' },
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
        order: [[1, 'asc']]
    });

    function clearForm() {
        $("#medicamentoId").val('');
        $("#medicamentoNombre").val('');
        $("#medicamentoDescripcion").val('');
        $("#medicamentoStock").val('');
        $("#medicamentoPrecio").val('');
        $("#medicamentoProveedorId").val('');
        medicamentoValidator.refresh();
    }

    $("#btnNuevoMedicamento").on('click', function () {
        isEdit = false;
        clearForm();
        $("#medicamentoModalTitle").text("Nuevo Medicamento");
        modal.show();
    });

    $('#medicamentoTable').on('click', '.btn-edit', async function () {
        const id = $(this).data('id');
        try {
            const response = await axios.post(`/Medicamento/GetByid?id=${id}`);
            isEdit = true;
            $('#medicamentoId').val(response.data.id);
            $('#medicamentoNombre').val(response.data.nombre);
            $('#medicamentoDescripcion').val(response.data.descripcion);
            $('#medicamentoStock').val(response.data.stock);
            $('#medicamentoPrecio').val(response.data.precio);
            $('#medicamentoProveedorId').val(response.data.proveedorId);
            $('#medicamentoModalTitle').text("Editar Medicamento");
            medicamentoValidator.refresh();
            modal.show();

        } catch {
            Swal.fire('Error', 'No se pudo cargar el medicamento', 'error');
        }
    });
    $('#medicamentoTable').on('click', '.btn-delete', async function () {
        const id = $(this).data('id');
        const nombre = $(this).data('nombre')
        const confirm = await Swal.fire({
            title: '¿Eliminar medicamento?',
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
            const response = await axios.post(`/Medicamento/Delete?id=${id}`);
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

    medicamentoValidator.onSuccess(async (event) => {
        event.preventDefault();

        const payload = {
            id: $('#medicamentoId').val() ? parseInt($('#medicamentoId').val()) : null,
            nombre: $('#medicamentoNombre').val().trim(),
            descripcion: $('#medicamentoDescripcion').val().trim(),
            stock: parseInt($('#medicamentoStock').val()),
            precio: parseFloat($('#medicamentoPrecio').val()),
            proveedorId: parseInt($('#medicamentoProveedorId').val())
        };

        try {
            const response = isEdit
                ? await axios.post(`/Medicamento/Update`, payload)
                : await axios.post(`/Medicamento/Create`, payload);
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
