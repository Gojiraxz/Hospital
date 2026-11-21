// Funciones de ayuda compartidas por los modulos del hospital (el _Layout ya incluye axios y SweetAlert2).

// Formatea fecha (dd/MM/yyyy)
function fmtDate(d) {
    if (!d) return '';
    const p = d.substring(0, 10).split('-');
    return `${p[2]}/${p[1]}/${p[0]}`;
}

// Formatea fecha y hora (dd/MM/yyyy HH:mm)
function fmtDateTime(d) {
    if (!d) return '';
    return `${fmtDate(d)} ${d.substring(11, 16)}`;
}

// Formatea dinero en cordobas
function money(d) {
    return d === null || d === undefined ? '' : `C$ ${Number(d).toFixed(2)}`;
}

// Etiqueta de color segun el estado
function badgeEstado(estado) {
    const e = (estado || '').toLowerCase();
    let css = 'estado-info';
    if (['confirmada', 'atendida', 'disponible', 'pagada', 'entregado', 'finalizada', 'aprobada'].includes(e)) css = 'estado-ok';
    else if (['pendiente', 'solicitado', 'programada', 'mantenimiento', 'en proceso'].includes(e)) css = 'estado-warn';
    else if (['cancelada', 'anulada', 'ocupada', 'rechazada'].includes(e)) css = 'estado-bad';
    return `<span class="badge-estado ${css}">${estado ?? ''}</span>`;
}
