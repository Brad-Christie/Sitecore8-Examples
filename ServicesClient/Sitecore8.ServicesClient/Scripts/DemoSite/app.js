(function (window, $, undefined) {
    'use strict';

    //
    // Bootstrap Alert Helper
    //
    $.fn.bsAlert = function (opts) {
        if (typeof opts === 'string') {
            opts = { 'message': opts };
        }
        var options = $.extend({}, $.fn.bsAlert.defaults, opts);

        var $alert = $('<div>').addClass('alert alert-' + options.type).html(options.message);
        if (options.preface) {
            $('<strong>').text(options.preface).prependTo($alert);
        }
        if (options.title) {
            $('<h4>').text(options.title).prependTo($alert);
        }
        if (options.closable && $.fn.alert) {
            $('<button>', {
                'attr': {
                    'type': 'button',
                    'class': 'close',
                    'aria-label': 'Close',
                    'data-dismiss': 'alert' // http://api.jquery.com/data/#data-html5 (all values are cached--can't use .data())
                },
                'html': $('<span>', {
                    'attr': {
                        'aria-hidden': 'true'
                    },
                    'html': '&times;'
                })
            }).prependTo($alert);
            $alert.alert();
        }
        $alert.appendTo(this);
    };
    $.fn.bsAlert.defaults = {
        'type': 'info',
        'preface': undefined,
        'title': undefined,
        'message': 'This is an alert.',
        'closable': true
    };

    $(function () {
        // "Presets" dropdowns
        $('.input-group').on('click', '.dropdown-menu a', function (e) {
            var $this = $(this),
                $value = $this.data('value'),
                $input = $this.closest('.input-group').find(':input');
            $input.val($value);
        });

        // Form preparation
        $('form').submit(function (e) {
            var $form = $(this),
                $target = $($form.data('target')),
                $button = $form.find('button[type="submit"]');
            $target.empty();
        });
    });
})(window, jQuery);