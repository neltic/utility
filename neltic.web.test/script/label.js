var Label = {
    Apply: "Aplicar",
    Change: "Cambiar",
    ConfirmAction: "¿Desea llevar acabo la acción?",
    ConfirmClear: "¿Desea limpiar todos los campos del formulario?",
    Create: "Crear",
    Go: "Ir",
    Import: "Importar",
    New: "Nuevo",
    Next: "Siguiente",
    NotEditable: "El registro no es editable",
    PageNotFound: "¡La pagina no se encuentra disponible!"
}
function applyLabel() { $('*').filter(function () { return $(this).data('label') !== undefined; }).each(function () { $(this).html(Label[$(this).data('label')]); }); }
$(function () { applyLabel(); });
