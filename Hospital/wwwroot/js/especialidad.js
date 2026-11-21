$(function () {
    const config = window.especialidadConfig || {};
    const modal = new bootstrap.Modal("#especialidadModal");
    let isEdit = false;

    // Validaciones del formulario con JustValidate
    const especialidadValidator = new JustValidate("#especialidadForm", {
        validateBeforeSubmitting: true,
        focusInvalidField: true,
        errorFieldCssClass: 'is-invalid',
        successFieldCssClass: 'is-valid'
    });

    especialidadValidator
        .addField("#especialidadNombre", [
            { rule: 'required', errorMessage: "El campo nombre es obligatorio." },
            { rule: 'minLength', value: 2, errorMessage: "Minimo 2 caracteres" },
            { rule: 'maxLength', value: 45, errorMessage: "Maximo es de 45 caracteres" },
        ]);

    // Tabla con paginacion, busqueda y orden en el servidor
    const table = $("#especialidadTable").DataTable({
        processing: true,
        serverSide: true,
        ajax: {
            url: '/Especialidad/GetData',
            type: 'POST'
        },
        columns: [
            { data: 'id' },
            { data: 'nombre' },
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
        $("#especialidadId").val('');
        $("#especialidadNombre").val('');
        especialidadValidator.refresh();
    }

    $("#btnNuevoEspecialidad").on('click', function () {
        isEdit = false;
        clearForm();
        $("#especialidadModalTitle").text("Nueva Especialidad");
        modal.show();
    });

    $('#especialidadTable').on('click', '.btn-edit', async function () {
        const id = $(this).data('id');
        try {
            const response = await axios.post(`/Especialidad/GetByid?id=${id}`);
            isEdit = true;
            $('#especialidadId').val(response.data.id);
            $('#especialidadNombre').val(response.data.nombre);
            $('#especialidadModalTitle').text("Editar Especialidad");
            especialidadValidator.refresh();
            modal.show();

        } catch {
            Swal.fire('Error', 'No se pudo cargar la especialidad', 'error');
        }
    });
    $('#especialidadTable').on('click', '.btn-delete', async function () {
        const id = $(this).data('id');
        const nombre = $(this).data('nombre')
        const confirm = await Swal.fire({
            title: '¿Eliminar especialidad?',
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
            const response = await axios.post(`/Especialidad/Delete?id=${id}`);
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

    especialidadValidator.onSuccess(async (event) => {
        event.preventDefault();

        const payload = {
            id: $('#especialidadId').val() ? parseInt($('#especialidadId').val()) : null,
            nombre: $('#especialidadNombre').val().trim()
        };

        try {
            const response = isEdit
                ? await axios.post(`/Especialidad/Update`, payload)
                : await axios.post(`/Especialidad/Create`, payload);
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
