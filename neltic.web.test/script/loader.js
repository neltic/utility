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
