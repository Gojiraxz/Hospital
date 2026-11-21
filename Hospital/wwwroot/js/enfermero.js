$(function () {
    const config = window.enfermeroConfig || {};
    const modal = new bootstrap.Modal("#enfermeroModal");
    let isEdit = false;

    // Validaciones del formulario con JustValidate
    const enfermeroValidator = new JustValidate("#enfermeroForm", {
        validateBeforeSubmitting: true,
        focusInvalidField: true,
        errorFieldCssClass: 'is-invalid',
        successFieldCssClass: 'is-valid'
    });

    enfermeroValidator
        .addField("#enfermeroNombre", [
            { rule: 'required', errorMessage: "El campo nombre es obligatorio." },
            { rule: 'minLength', value: 2, errorMessage: "Minimo 2 caracteres" },
            { rule: 'maxLength', value: 45, errorMessage: "Maximo es de 45 caracteres" },
        ])
        .addField("#enfermeroApellido", [
            { rule: 'required', errorMessage: "El campo apellido es obligatorio." },
            { rule: 'minLength', value: 2, errorMessage: "Minimo 2 caracteres" },
            { rule: 'maxLength', value: 45, errorMessage: "Maximo es de 45 caracteres" },
        ])
        .addField("#enfermeroEspecialidadId", [
            { rule: 'required', errorMessage: "Seleccione especialidad." },
        ]);

    // Tabla con paginacion, busqueda y orden en el servidor
    const table = $("#enfermeroTable").DataTable({
        processing: true,
        serverSide: true,
        ajax: {
            url: '/Enfermero/GetData',
            type: 'POST'
        },
        columns: [
            { data: 'id' },
            { data: 'nombre' },
            { data: 'apellido' },
            { data: 'especialidadName' },
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
                        data-id="${data.id}" data-nombre="${data.nombre} ${data.apellido}">
                        <i class="fa-solid fa-trash"></i></button>`;
                    }
                    return buttons || '<span class="text-muted">-</span>'
                }
            }
        ],
        language: {
            url: 'https://cdn.datatables.net/plug-ins/1.13.8/i18n/es-ES.json'
        },
        order: [[2, 'asc']]
    });

    function clearForm() {
        $("#enfermeroId").val('');
        $("#enfermeroNombre").val('');
        $("#enfermeroApellido").val('');
        $("#enfermeroEspecialidadId").val('');
        enfermeroValidator.refresh();
    }

    $("#btnNuevoEnfermero").on('click', function () {
        isEdit = false;
        clearForm();
        $("#enfermeroModalTitle").text("Nuevo Enfermero");
        modal.show();
    });

    $('#enfermeroTable').on('click', '.btn-edit', async function () {
        const id = $(this).data('id');
        try {
            const response = await axios.post(`/Enfermero/GetByid?id=${id}`);
            isEdit = true;
            $('#enfermeroId').val(response.data.id);
            $('#enfermeroNombre').val(response.data.nombre);
            $('#enfermeroApellido').val(response.data.apellido);
            $('#enfermeroEspecialidadId').val(response.data.especialidadId);
            $('#enfermeroModalTitle').text("Editar Enfermero");
            enfermeroValidator.refresh();
            modal.show();

        } catch {
            Swal.fire('Error', 'No se pudo cargar el enfermero', 'error');
        }
    });
    $('#enfermeroTable').on('click', '.btn-delete', async function () {
        const id = $(this).data('id');
        const nombre = $(this).data('nombre')
        const confirm = await Swal.fire({
            title: '¿Eliminar enfermero?',
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
            const response = await axios.post(`/Enfermero/Delete?id=${id}`);
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

    enfermeroValidator.onSuccess(async (event) => {
        event.preventDefault();

        const payload = {
            id: $('#enfermeroId').val() ? parseInt($('#enfermeroId').val()) : null,
            nombre: $('#enfermeroNombre').val().trim(),
            apellido: $('#enfermeroApellido').val().trim(),
            especialidadId: parseInt($('#enfermeroEspecialidadId').val())
        };

        try {
            const response = isEdit
                ? await axios.post(`/Enfermero/Update`, payload)
                : await axios.post(`/Enfermero/Create`, payload);
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
