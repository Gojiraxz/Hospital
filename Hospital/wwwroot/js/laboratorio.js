$(function () {
    const config = window.laboratorioConfig || {};
    const modal = new bootstrap.Modal("#laboratorioModal");
    let isEdit = false;

    // Validaciones del formulario con JustValidate
    const laboratorioValidator = new JustValidate("#laboratorioForm", {
        validateBeforeSubmitting: true,
        focusInvalidField: true,
        errorFieldCssClass: 'is-invalid',
        successFieldCssClass: 'is-valid'
    });

    laboratorioValidator
        .addField("#laboratorioNombre", [
            { rule: 'required', errorMessage: "El campo nombre es obligatorio." },
            { rule: 'minLength', value: 2, errorMessage: "Minimo 2 caracteres" },
            { rule: 'maxLength', value: 45, errorMessage: "Maximo es de 45 caracteres" },
        ])
        .addField("#laboratorioUbicacion", [
            { rule: 'required', errorMessage: "El campo ubicación es obligatorio." },
            { rule: 'minLength', value: 2, errorMessage: "Minimo 2 caracteres" },
            { rule: 'maxLength', value: 45, errorMessage: "Maximo es de 45 caracteres" },
        ])
        .addField("#laboratorioTelefono", [
            { rule: 'required', errorMessage: "El campo teléfono es obligatorio." },
            { rule: 'minLength', value: 2, errorMessage: "Minimo 2 caracteres" },
            { rule: 'maxLength', value: 15, errorMessage: "Maximo es de 15 caracteres" },
        ]);

    // Tabla con paginacion, busqueda y orden en el servidor
    const table = $("#laboratorioTable").DataTable({
        processing: true,
        serverSide: true,
        ajax: {
            url: '/Laboratorio/GetData',
            type: 'POST'
        },
        columns: [
            { data: 'id' },
            { data: 'nombre' },
            { data: 'ubicacion' },
            { data: 'telefono' },
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
        $("#laboratorioId").val('');
        $("#laboratorioNombre").val('');
        $("#laboratorioUbicacion").val('');
        $("#laboratorioTelefono").val('');
        laboratorioValidator.refresh();
    }

    $("#btnNuevoLaboratorio").on('click', function () {
        isEdit = false;
        clearForm();
        $("#laboratorioModalTitle").text("Nuevo Laboratorio");
        modal.show();
    });

    $('#laboratorioTable').on('click', '.btn-edit', async function () {
        const id = $(this).data('id');
        try {
            const response = await axios.post(`/Laboratorio/GetByid?id=${id}`);
            isEdit = true;
            $('#laboratorioId').val(response.data.id);
            $('#laboratorioNombre').val(response.data.nombre);
            $('#laboratorioUbicacion').val(response.data.ubicacion);
            $('#laboratorioTelefono').val(response.data.telefono);
            $('#laboratorioModalTitle').text("Editar Laboratorio");
            laboratorioValidator.refresh();
            modal.show();

        } catch {
            Swal.fire('Error', 'No se pudo cargar el laboratorio', 'error');
        }
    });
    $('#laboratorioTable').on('click', '.btn-delete', async function () {
        const id = $(this).data('id');
        const nombre = $(this).data('nombre')
        const confirm = await Swal.fire({
            title: '¿Eliminar laboratorio?',
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
            const response = await axios.post(`/Laboratorio/Delete?id=${id}`);
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

    laboratorioValidator.onSuccess(async (event) => {
        event.preventDefault();

        const payload = {
            id: $('#laboratorioId').val() ? parseInt($('#laboratorioId').val()) : null,
            nombre: $('#laboratorioNombre').val().trim(),
            ubicacion: $('#laboratorioUbicacion').val().trim(),
            telefono: $('#laboratorioTelefono').val().trim()
        };

        try {
            const response = isEdit
                ? await axios.post(`/Laboratorio/Update`, payload)
                : await axios.post(`/Laboratorio/Create`, payload);
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
