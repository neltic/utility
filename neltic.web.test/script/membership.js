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
