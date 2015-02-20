/*
    by @neltic on 2014-12-01
    need jQuery v1.10+

    Bind utilities
*/
function bindList(selectId, source, dataID, dataName, defaultSelected) {
    if (typeof (defaultSelected) !== 'number') { defaultSelected = -1; }
    $(selectId).find('option').remove();
    $.each(source, function (key, item) {
        var props = { value: item[dataID] };
        if (defaultSelected == item[dataID]) {
            props.selected = true;
        }
        var text = item[dataName];
        $(selectId).append($('<option>', props).text(text));
    });
}

function bindUI(obj, context, prefix, onlyReturnChanges) {
    if (typeof (onlyReturnChanges) !== 'boolean') { onlyReturnChanges = false; }
    for (var property in obj) {
        var name = property;
        var $search = $('#' + prefix + name, context);
        if ($search.length > 0) {
            if ($search.is('input') || $search.is('textarea') || $search.is('select')) {
                switch ($search.prop('type')) {
                    case 'checkbox':
                        $search.prop('checked', obj[property]);
                        break;
                    default:
                        $search.val(obj[property]);
                        break;
                }
            } else {
                $search.html(obj[property]);
            }
        } else if (onlyReturnChanges) {
            delete obj[property];
        }
    }
}

function bindObject(obj, context, prefix, onlyReturnChanges) {
    if (typeof (onlyReturnChanges) !== 'boolean') { onlyReturnChanges = false; }
    for (var property in obj) {
        var name = property;
        var $search = $('#' + prefix + name, context);
        if ($search.length > 0) {
            if ($search.is('input') || $search.is('textarea') || $search.is('select')) {
                switch ($search.prop('type')) {
                    case 'checkbox':
                        obj[property] = $search.prop('checked');
                        break;
                    default:
                        obj[property] = parseValueBy(typeof obj[property], $search.val());
                        break;
                }
            } else {
                obj[property] = parseValueBy(typeof obj[property], $search.html());
            }
        } else if (onlyReturnChanges) {
            delete obj[property];
        }
    }
    return obj;
}

function bindDataUI(obj, $context, prefix) {
    if ($context.length > 0) {
        if (typeof (prefix) !== 'string') { prefix = ''; }
        for (var property in obj) {
            var name = property.toLowerCase();
            $context.data(prefix + name, obj[property]);
        }
    }
}

function bindDataObject(obj, $context, prefix) {
    if ($context.length > 0) {
        if (typeof (prefix) !== 'string') { prefix = ''; }
        for (var property in obj) {
            var name = property.toLowerCase();
            obj[property] = parseValueBy(typeof obj[property], $context.data(prefix + name));
        }
    }
    return obj;
}

function parseValueBy(typeOfProperty, val) {
    var value = val;
    switch (typeOfProperty) {
        case 'number':
            value = parseInt(val);
            if (value % 1 !== 0) {
                value = parseFloat(val);
            }
            break;
        case 'boolean':
            value = val === 'true' || val === true ? true : false;
            break;
        case 'object':
            if (val === '') { // may be null 
                value = null;
            }
            break;
    }
    return value;
}

/* old web service function */
// extiendo jQuery para el manejo de webmethods :)
jQuery.WebMethod = function (urlMethod, jsonData, callback) {
    var xhr = null;
    // no se incluyo data
    if (typeof (jsonData) == "function") { callback = jsonData; jsonData = "{}"; }
    // no se incluyo o no es valido el callback
    if (typeof (callback) != "function") { callback = function (result) { }; }
    // llamamos web method
    if (continueMission) {
        xhr = $.ajax({
            type: "POST",
            url: urlMethod,
            data: jsonData,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (result) { callback(result.d); },
            error: function (x, e, errorThrown) {
                if (x.status == 0 && e != "abort") {
                    alert("Al parecer aún no se cargaba información que se solicitó previamente, si la información ya no es requerida haga caso omiso de este mensaje.");
                } else if (x.status == 404) {
                    alert("Pagina no encontrada");
                } else if (x.status == 500) {
                    alert("Al parecer la red no esta disponible o responde muy lentamente, si el cambio no se ve reflejado intente actualizar manualmente pulsando F5.\n\nSi el problema persiste consulte con el administrador del sitio.\n\nInformación de referencia para el área de TI:\n" + urlMethod + "\n" + jsonData);
                } else if (e == "parsererror") {
                    alert("Error procesando la respuesta del servidor [JSON].");
                } else if (e == "timeout") {
                    alert("Tiempo de espera agotado");
                } else if (e != "abort") {
                    alert("La red se encuentra intermitente."); // se comento por que al asuario le "hace ruido" 
                }
            }
        });
        return xhr;
    }
}