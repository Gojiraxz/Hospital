$(function () {
    const config = window.doctorConfig || {};
    const modal = new bootstrap.Modal("#doctorModal");
    let isEdit = false;

    // Validaciones del formulario con JustValidate
    const doctorValidator = new JustValidate("#doctorForm", {
        validateBeforeSubmitting: true,
        focusInvalidField: true,
        errorFieldCssClass: 'is-invalid',
        successFieldCssClass: 'is-valid'
    });

    doctorValidator
        .addField("#doctorNombre", [
            { rule: 'required', errorMessage: "El campo nombre es obligatorio." },
            { rule: 'minLength', value: 2, errorMessage: "Minimo 2 caracteres" },
            { rule: 'maxLength', value: 50, errorMessage: "Maximo es de 50 caracteres" },
        ])
        .addField("#doctorApellido", [
            { rule: 'required', errorMessage: "El campo apellido es obligatorio." },
            { rule: 'minLength', value: 2, errorMessage: "Minimo 2 caracteres" },
            { rule: 'maxLength', value: 50, errorMessage: "Maximo es de 50 caracteres" },
        ])
        .addField("#doctorEspecialidadId", [
            { rule: 'required', errorMessage: "Seleccione especialidad." },
        ]);

    // Tabla con paginacion, busqueda y orden en el servidor
    const table = $("#doctorTable").DataTable({
        processing: true,
        serverSide: true,
        ajax: {
            url: '/Doctor/GetData',
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
        $("#doctorId").val('');
        $("#doctorNombre").val('');
        $("#doctorApellido").val('');
        $("#doctorEspecialidadId").val('');
        doctorValidator.refresh();
    }

    $("#btnNuevoDoctor").on('click', function () {
        isEdit = false;
        clearForm();
        $("#doctorModalTitle").text("Nuevo Doctor");
        modal.show();
    });

    $('#doctorTable').on('click', '.btn-edit', async function () {
        const id = $(this).data('id');
        try {
            const response = await axios.post(`/Doctor/GetByid?id=${id}`);
            isEdit = true;
            $('#doctorId').val(response.data.id);
            $('#doctorNombre').val(response.data.nombre);
            $('#doctorApellido').val(response.data.apellido);
            $('#doctorEspecialidadId').val(response.data.especialidadId);
            $('#doctorModalTitle').text("Editar Doctor");
            doctorValidator.refresh();
            modal.show();

        } catch {
            Swal.fire('Error', 'No se pudo cargar el doctor', 'error');
        }
    });
    $('#doctorTable').on('click', '.btn-delete', async function () {
        const id = $(this).data('id');
        const nombre = $(this).data('nombre')
        const confirm = await Swal.fire({
            title: '¿Eliminar doctor?',
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
            const response = await axios.post(`/Doctor/Delete?id=${id}`);
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

    doctorValidator.onSuccess(async (event) => {
        event.preventDefault();

        const payload = {
            id: $('#doctorId').val() ? parseInt($('#doctorId').val()) : null,
            nombre: $('#doctorNombre').val().trim(),
            apellido: $('#doctorApellido').val().trim(),
            especialidadId: parseInt($('#doctorEspecialidadId').val())
        };

        try {
            const response = isEdit
                ? await axios.post(`/Doctor/Update`, payload)
                : await axios.post(`/Doctor/Create`, payload);
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
