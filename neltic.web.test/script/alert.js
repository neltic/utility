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
