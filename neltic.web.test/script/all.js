"use strict";
/*
   by @neltic on 2015-01-01
   need jQuery v1.10+

   Create a loader message
*/
var loader = {
    id: 'loader', // CSS Id
    template: '<div>{0}</div>', // message template
    isBusy: false,
    delay: 1000, // until this time the message is displayed
    show: function (message) {
        var $loader = $('#' + this.id);
        if ($('#' + this.id).length === 0) {
            $loader = $('<div id="' + this.id + '"><div><p></p></div></div>');
            $('body').append($loader);
        }
        // create loading message
        var $current = $(this.template.replace('{0}', message.replace(/\n/g, '<br/>')));
        $current.insertBefore($loader.find('p'));
        // check is time to... 
        if (!this.isBusy) {
            this.isBusy = true;
            setTimeout(function () {
                if (loader.isBusy) {
                    $('#' + loader.id).fadeIn();
                }
            }, this.delay);
        }
        this.center();
    },
    hide: function () {
        this.isBusy = false;
        $('#' + this.id).fadeOut("slow");
        this.clear();
    },
    center: function () {
        var w = $(window).width() + $(window).scrollLeft();
        var h = $(window).height();
        if (navigator.appName == 'Microsoft Internet Explorer') {
            w = $(document).width() + $(document).scrollLeft();
            h = $(document).height();
        }
        $('#' + this.id).width(w);
        $('#' + this.id).height(h);
        $('#' + this.id + " > div").css("margin-top", (h / 2) - 60);
    },
    clear: function () {
        var $messages = $('#' + this.id + ' > div > div');
        if ($messages.length > 0) {
            $messages.remove();
        }
    }
};
/*
    Requires a css code like

    #loader { display: none; width: 100%; height: 50px; position: absolute; top: 0; left: 0; background-color: rgba(0,0,0,.8); text-align: center; z-index: 999999; }
    #loader > div { margin: 0 auto; min-height: 50px; position: relative; padding: 12px; width: 300px; height: auto; background-color: rgba(0, 0, 0, .5); border-radius: 3px; border: solid 1px #93DB0A; }
    #loader > div > div { display: block; color: #fff; font-weight:bold; font-size: 12px; }
    #loader > div > p { display: block; width: 120px; height: 12px; margin: 0 auto; border-radius: 2px; border: dotted 1px #333; margin-top: 8px; background: #93DB0A url(../image/ui/animated-overlay.gif) repeat-x left center; }
*/
/*
   by @neltic on 2015-01-01
   need Bootstrap & jQuery v1.10+

   Create an alert as toast-message
*/
var alert = function (type, message, title, configuration) {
    // default configuration
    var config = {
        id: 'alert', // CSS Id
        auto: true,
        time: 5000,
        template: '<div class="alert alert-{0} fade in" role="alert"><span class="glyphicon glyphicon-{1}" aria-hidden="true"></span><strong class="alert-title">{2}</strong><button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button>{3}</div>',
        icons: {
            success: 'ok-sign',
            info: 'question-sign',
            warning: 'exclamation-sign',
            danger: 'remove-sign'
        }
    };
    // 'error' as default alert... is assumed to
    if (!(type in config.icons)) { configuration = title; title = message; message = type; type = 'danger'; config.auto = false; }
    // is assumed to be the configuration...
    if (typeof (title) === 'object') { configuration = title; title = null; }
    if (typeof (configuration) === 'object') { $.extend(config, configuration); }
    if (typeof (message) !== 'string') { message = ''; }
    if (typeof (title) !== 'string') { var all = message.split('\n'); title = all.shift(); message = all.join('\n'); }
    // init dom
    var $holder = $('#' + config.id);
    if ($('#' + config.id).length === 0) {
        $holder = $('<div id="' + config.id + '"></div>');
        $('body').append($holder);
        $holder.css('left', Math.max(0, (($(document).width() - $('#' + config.id).outerWidth()) / 2) + $(document).scrollLeft()) + 'px');
    }
    // create toast-message
    var $current = $(config.template.replace('{0}', type).replace('{1}', config.icons[type]).replace('{2}', title.replace(/\n/g, '<br/>')).replace('{3}', message !== '' ? ('<br/><br/>' + message.replace(/\n/g, '<br/>')) : ''));
    $holder.append($current);
    // auto hide
    if (config.auto) {
        setTimeout(function () {
            $current.alert('close');
        }, config.time);
    }
}
/*
    Requires a css code like
    
    #alert { z-index: 999999; position: absolute; top: -4px; margin: 0 auto; width: 70%; padding: 8px;  }
    #alert > div { margin-top:8px; }
    #alert .alert-title { margin-left:16px; }
*/
/*
   by @neltic on 2014-12-01
   need jQuery v1.10+
*/
var dateRegExp = new RegExp(/\/Date\(([0-9-]+)\)\//);
/*
    Javascript class 'ServiceResponse' may need a 'ServiceResponse' C# Class/Object  
*/
var ServiceResponse = function (config) {

    this.configuration = {
        endpoint: "",
        defaultBodyStyle: 1,
        warningAsError: false,
        onError: function (message, status) {
            alert(message, "Error " + status);
        },
        onStatus: {
            401: function (context, result) {
                alert(result.Message);
            }
        }
    };
    this.caller = {
        // store caller info
    };
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
        context.store('get', method, data, onSuccess);
        $.ajax({
            type: 'GET',
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            url: context.configuration.endpoint + method + (data != null ? ('?' + $.param(data)) : ''),
            success: function (response) { context.processResponse(response, onSuccess); },
            error: function (xhr, textStatus, errorThrown) {
                context.configuration.onError(textStatus + '\n- ' + errorThrown, xhr.status);
            }
        });
    };
    this.post = function (method, data, onSuccess) {
        if (typeof (data) === 'function') { onSuccess = data; data = {}; }
        var context = this;
        context.store('post', method, data, onSuccess);
        $.ajax({
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            data: typeof (data) === 'string' ? data : JSON.stringify(data),
            url: context.configuration.endpoint + method,
            success: function (response) { context.processResponse(response, onSuccess); },
            error: function (xhr, textStatus, errorThrown) {
                context.configuration.onError(textStatus + '\n- ' + errorThrown, xhr.status);
            }
        });
    };
    this.store = function (invoke, method, data, onSuccess) {
        this.caller = {
            invoke: invoke,
            method: method,
            data: data,
            onSuccess: onSuccess
        }
    };
    this.processResponse = function (response, onSuccess) {
        var context = this;
        var result = response;
        if ((context.configuration.defaultBodyStyle & 2) == 2) { result = response[method + 'Result']; }
        if (!(!result.Status && !result.Message && !result.Response)) {
            if (typeof (context.configuration.onStatus[result.Status]) !== 'function') {
                if ((result.Status & 200) === 200 || !context.configuration.warningAsError) {
                    result.Response == parseJSONObject(result.Response);
                    onSuccess(result.Response, result.Message, result.Status);
                } else {
                    context.configuration.onError(result.Message, result.Status);
                }
            } else {
                context.configuration.onStatus[result.Status](context, result);
            }
        } else {
            onSuccess(parseJSONObject(result));
        }
    };
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
/*
   by @neltic on 2015-01-01
   need        
       jQuery v1.10+
       Bootstrap
       ServiceResponse
       applyLabel for 'Label' object
           & 'Label'.Validating
       loader

   Create an authenticator form
*/
var membership = {
    id: 'auth', // CSS Id
    template: '<div class="modal fade in" id="{0}" data-show="true" data-keyboard="false" data-backdrop="static" tabindex="-1" role="dialog" aria-labelledby="loginLabel" aria-hidden="true" style="display: none;"><div class="modal-dialog modal-sm"><div class="modal-content"><div class="modal-header"><h3 class="modal-title"><span class="glyphicon glyphicon-user" aria-hidden="true"></span><span class="has-icon-left" id="loginLabel" data-label="Login"></span></h3></div><div class="modal-body"><form class="form-horizontal"><div class="form-group"><label class="control-label col-sm-4" data-label="User"></label><div class="col-sm-8"><input id="user" type="text" class="form-control"></div></div><div class="form-group"><label class="control-label col-sm-4" data-label="Password"></label><div class="col-sm-8"><input id="pass" type="password" class="form-control"></div></div></form></div><div class="modal-footer"><button type="button" class="btn btn-primary"><span class="glyphicon glyphicon-log-in" aria-hidden="true"></span><span class="has-icon-left" data-label="Authenticate"></span></button></div></div></div></div>',
    render: applyLabel,
    userService: new ServiceResponse({
        endpoint: '/school/rest/UserService.svc/json/'  // don't forget to set your own (use semi absolute path)
    }),
    login: function (onReady) {
        loader.hide(); // if is used for a ajax
        var $auth = $('#' + this.id);
        if ($auth.length == 0) {
            $auth = $(this.template.replace('{0}', this.id));
            $('body').append($auth);
        }
        this.render($auth);
        // create modal
        $auth.modal('show');
        var $authbtn = $auth.find('.btn-primary');
        $authbtn.prop('disabled', null);
        // attach auth event
        $auth.find('.btn-primary').off('click');
        $auth.find('.btn-primary').on('click', function () {
            $(this).prop('disabled', 'disabled');
            loader.show(Label.Validating);
            var data = { alias: $('#user').val(), pass: $('#pass').val() };
            membership.userService.post('Authenticate', data, function (response, message, status) {
                loader.hide();
                if (response) { // is authenticated
                    membership.hide();
                    onReady();
                } else {
                    membership.clear();
                    alert('warning', message);
                }
            });
        });
        this.center();
    },
    hide: function () {
        this.clear();
        $('#' + this.id).modal('hide');
    },
    center: function () {
        var h = $(window).height();
        if (navigator.appName == 'Microsoft Internet Explorer') {
            h = $(document).height();
        }
        $('.modal-content', '#' + this.id).css("margin-top", (h / 2) - 120);
    },
    clear: function () {
        $('input[type="password"]', '#' + this.id).val('');
        $('.btn-primary', '#' + this.id).prop('disabled', null);
    },
    /* To use width service.response.js */
    onStatus: {
        401: function (context, result) {
            alert('info', Label.SessionLost);
            // vamos a intentar autenticar al usuario
            membership.login(function () {
                loader.show('...');
                context[context.caller.invoke](context.caller.method, context.caller.data, context.caller.onSuccess);
            });
        }
    }
}
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
