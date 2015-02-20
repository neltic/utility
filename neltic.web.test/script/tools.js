var continueMission = true;

// parse json date
function parseJSONDate(date) {
    try { return new Date(parseInt(date.replace(/\/Date\((-?\d+)\)\//, '$1'))); }
    catch (e) { return null; }
}

function toEvalDate(date) {
    try {
        var d = date.split('-');
        if (d.length > 2) {
            d[1] = parseInt(d[1]) - 1;
            return "new Date(" + d.join(',') + ")";
        }
        return null;
    }
    catch (e) { return null; }
}

function toEvalExactDate(date) {
    try {
        var d = date.split('-');
        return "new Date(" + d.join(',') + ")";
    }
    catch (e) { return null; }
}

function toLongDate(date) {
    try { return toShortDate(date) + " " + toSQLTime(date); }
    catch (e) { return null; }
}

function toShortDate(date) {
    try { return fillZD(date.getDate()) + "/" + fillZD(date.getMonth() + 1) + "/" + date.getFullYear(); }
    catch (e) { return null; }
}

function toSQLDate(date) {
    try { return date.getFullYear() + "-" + fillZD(date.getMonth() + 1) + "-" + fillZD(date.getDate()); }
    catch (e) { return null; }
}

function toSQLTime(date) {
    try { return fillZD(date.getHours()) + ":" + fillZD(date.getMinutes()) + ":" + fillZD(date.getSeconds()); }
    catch (e) { return null; }
}

function toFriendlyDate(date) {
    var monthNames = ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'];
    var d = date.getHours();
    var mp = "am";
    if (d > 12) { d -= 12; mp = "pm"; }
    try { return date.getDate() + " de " + monthNames[date.getMonth()] + " de " + date.getFullYear() + " a las " + d + ":" + fillZD(date.getMinutes()) + " " + mp; }
    catch (e) { return null; }
}

function fillZD(number) {
    if (number < 10) return "0" + number;
    else return number;
}

function normalize(str) {
    try { return str.replace(/\\/g, "\\\\").replace(/"/g, "\\\"").replace(/\n/g, "\\n").replace(/\t/g, "\\t"); }
    catch (e) { return null; }
}

function replaceAll(str, pattern, newtext) {
    try {
        rex = new RegExp(pattern, "g");
        return str.replace(rex, newtext);
    } catch (e) {
        return "";
    }
}

function toCurrency(data) {
    var value = parseFloat(replaceAll(data, ',', ''));
    if (!isFinite(value)) value = 0;
    return value.toFixed(global.money.decimal);
}

function dateDiffInDays(a, b) {
    var MS_PER_DAY = 86400000; // 1000 * 60 * 60 * 24;
    var utc1 = Date.UTC(a.getFullYear(), a.getMonth(), a.getDate());
    var utc2 = Date.UTC(b.getFullYear(), b.getMonth(), b.getDate());
    return Math.floor((utc2 - utc1) / MS_PER_DAY);
}
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

