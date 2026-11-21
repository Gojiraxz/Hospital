$(function () {
    const config = window.proveedorConfig || {};
    const modal = new bootstrap.Modal("#proveedorModal");
    let isEdit = false;

    // Validaciones del formulario con JustValidate
    const proveedorValidator = new JustValidate("#proveedorForm", {
        validateBeforeSubmitting: true,
        focusInvalidField: true,
        errorFieldCssClass: 'is-invalid',
        successFieldCssClass: 'is-valid'
    });

    proveedorValidator
        .addField("#proveedorNombre", [
            { rule: 'required', errorMessage: "El campo nombre es obligatorio." },
            { rule: 'minLength', value: 2, errorMessage: "Minimo 2 caracteres" },
            { rule: 'maxLength', value: 45, errorMessage: "Maximo es de 45 caracteres" },
        ])
        .addField("#proveedorCorreo", [
            { rule: 'required', errorMessage: "El campo correo es obligatorio." },
            { rule: 'email', errorMessage: "Ingrese un correo valido" },
            { rule: 'minLength', value: 2, errorMessage: "Minimo 2 caracteres" },
            { rule: 'maxLength', value: 100, errorMessage: "Maximo es de 100 caracteres" },
        ])
        .addField("#proveedorCofepris", [
            { rule: 'required', errorMessage: "El campo registro sanitario (cofepris) es obligatorio." },
            { rule: 'minLength', value: 2, errorMessage: "Minimo 2 caracteres" },
            { rule: 'maxLength', value: 45, errorMessage: "Maximo es de 45 caracteres" },
        ]);

    // Tabla con paginacion, busqueda y orden en el servidor
    const table = $("#proveedorTable").DataTable({
        processing: true,
        serverSide: true,
        ajax: {
            url: '/Proveedor/GetData',
            type: 'POST'
        },
        columns: [
            { data: 'id' },
            { data: 'nombre' },
            { data: 'correo' },
            { data: 'cofepris' },
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
        $("#proveedorId").val('');
        $("#proveedorNombre").val('');
        $("#proveedorCorreo").val('');
        $("#proveedorCofepris").val('');
        proveedorValidator.refresh();
    }

    $("#btnNuevoProveedor").on('click', function () {
        isEdit = false;
        clearForm();
        $("#proveedorModalTitle").text("Nuevo Proveedor");
        modal.show();
    });

    $('#proveedorTable').on('click', '.btn-edit', async function () {
        const id = $(this).data('id');
        try {
            const response = await axios.post(`/Proveedor/GetByid?id=${id}`);
            isEdit = true;
            $('#proveedorId').val(response.data.id);
            $('#proveedorNombre').val(response.data.nombre);
            $('#proveedorCorreo').val(response.data.correo);
            $('#proveedorCofepris').val(response.data.cofepris);
            $('#proveedorModalTitle').text("Editar Proveedor");
            proveedorValidator.refresh();
            modal.show();

        } catch {
            Swal.fire('Error', 'No se pudo cargar el proveedor', 'error');
        }
    });
    $('#proveedorTable').on('click', '.btn-delete', async function () {
        const id = $(this).data('id');
        const nombre = $(this).data('nombre')
        const confirm = await Swal.fire({
            title: '¿Eliminar proveedor?',
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
            const response = await axios.post(`/Proveedor/Delete?id=${id}`);
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

    proveedorValidator.onSuccess(async (event) => {
        event.preventDefault();

        const payload = {
            id: $('#proveedorId').val() ? parseInt($('#proveedorId').val()) : null,
            nombre: $('#proveedorNombre').val().trim(),
            correo: $('#proveedorCorreo').val().trim(),
            cofepris: $('#proveedorCofepris').val().trim()
        };

        try {
            const response = isEdit
                ? await axios.post(`/Proveedor/Update`, payload)
                : await axios.post(`/Proveedor/Create`, payload);
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
