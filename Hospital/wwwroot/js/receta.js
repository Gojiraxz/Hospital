$(function () {
    const config = window.recetaConfig || {};
    const modal = new bootstrap.Modal("#recetaModal");

    const table = $("#recetaTable").DataTable({
        processing: true,
        serverSide: true,
        ajax: { url: '/Receta/GetData', type: 'POST' },
        columns: [
            { data: 'id' },
            { data: 'fechaSolicitud', render: d => fmtDateTime(d) },
            { data: 'pacienteName' },
            { data: 'pacienteCedula', orderable: false },
            { data: 'doctorName' },
            { data: 'medicamento' },
            { data: 'estado', render: d => badgeEstado(d) },
            {
                data: null,
                orderable: false,
                searchable: false,
                render: function () {
                    return config.canUpdate
                        ? `<button class="btn btn-sm btn-outline-primary btn-responder" title="Responder">
                               <i class="fa-solid fa-pen-to-square"></i></button>`
                        : '<span class="text-muted">-</span>';
                }
            }
        ],
        language: { url: 'https://cdn.datatables.net/plug-ins/1.13.8/i18n/es-ES.json' },
        order: [[1, 'desc']]
    });

    $('#recetaTable').on('click', '.btn-responder', function () {
        const row = table.row($(this).closest('tr')).data();
        $('#recetaId').val(row.id);
        $('#recetaPaciente').text(`${row.pacienteName} (${row.pacienteCedula})`);
        $('#recetaDoctor').text(row.doctorName);
        $('#recetaMedicamento').text(row.medicamento);
        $('#recetaIndicaciones').text(row.indicaciones);
        $('#recetaEstado').val(row.estado);
        $('#recetaRespuesta').val(row.respuesta || '');
        modal.show();
    });

    $('#recetaForm').on('submit', async function (e) {
        e.preventDefault();
        const payload = {
            id: parseInt($('#recetaId').val()),
            estado: $('#recetaEstado').val(),
            respuesta: $('#recetaRespuesta').val().trim()
        };
        try {
            const response = await axios.post('/Receta/Responder', payload);
            if (response.data.success) {
                modal.hide();
                table.ajax.reload(null, false);
                Swal.fire({ title: 'Listo', text: response.data.message, icon: 'success', timer: 2000, showConfirmButton: false });
            }
        } catch (error) {
            Swal.fire('Error', error.response?.data?.message || 'No se pudo guardar.', 'error');
        }
    });
});
