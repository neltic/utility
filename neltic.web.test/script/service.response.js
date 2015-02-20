"use strict";
/*
    by @neltic on 2014-12-01
    need jQuery v1.10+
*/
var dateRegExp = new RegExp(/\/Date\(([0-9-]+)\)\//);
/*
    Javascript class 'ServiceResponse' will need a 'ServiceResponse' C# Class/Object  
*/
var ServiceResponse = function (config) {

    this.configuration = {
        endpoint: "",
        defaultBodyStyle: 1,
        warningAsError: false,
        onError: function (status, message) {
            alert(message, "Error " + status);
        }
    }
    $.extend(true, this.configuration, config);
    /*  defaultBodyStyle
    Request     Response    BosyStyle               Comment    
    0           0           0 - Bare	            Both requests and responses are not wrapped.
    0           1           1 - WrappedRequest	    Requests are wrapped, responses are not wrapped.
    1           0           2 - WrappedResponse	    Responses are wrapped, requests are not wrapped.
    1           1           3 - Wrapped	            Both requests and responses are wrapped.
    */
    this.get = function (method, data, onSuccess) {
        if (typeof (data) === 'function') { onSuccess = data; data = null; }
        var context = this;
        $.ajax({
            type: 'GET',
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            url: context.configuration.endpoint + method + (data != null ? ('?' + $.param(data)) : ''),
            success: function (response) { context.processResponse(response, onSuccess); },
            error: function (xhr, textStatus, errorThrown) {
                context.configuration.onError(xhr.status, textStatus + '\n- ' + errorThrown);
            }
        });
    }
    this.post = function (method, data, onSuccess) {
        if (typeof (data) === 'function') { onSuccess = data; data = {}; }
        var context = this;
        $.ajax({
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            data: JSON.stringify(data),
            url: context.configuration.endpoint + method,
            success: function (response) { context.processResponse(response, onSuccess); },
            error: function (xhr, textStatus, errorThrown) {
                context.configuration.onError(xhr.status, textStatus + '\n- ' + errorThrown);
            }
        });
    }
    this.processResponse = function (response, onSuccess) {
        var context = this;
        var result = response;
        if ((context.configuration.defaultBodyStyle & 2) == 2) { result = response[method + 'Result']; }
        if (!(!result.Status && !result.Message && !result.Response)) {
            if ((result.Status & 200) === 200 || !context.configuration.warningAsError) {
                result.Response == parseJSONObject(result.Response);
                onSuccess(result.Response, result.Status, result.Message);
            } else {
                context.configuration.onError(result.Status, result.Message);
            }
        } else {
            onSuccess(parseJSONObject(result));
        }
    }
}

function parseJSONObject(obj) {
    for (var property in obj) {
        if (typeof obj[property] === 'object') {
            obj[property] = parseJSONObject(obj[property]);
        } else if (dateRegExp.test(obj[property])) {
            obj[property] = new Date(parseInt(obj[property].replace(dateRegExp, '$1')))
        }
    }
    return obj;
}
