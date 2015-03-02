(function (window, $, undefined) {
    'use strict';

    $(function () {
        // GET
        $('#custom-form').on('submit', function (e) {
            var $form = $(this),
                id = encodeURIComponent($('#custom-id').val()),
                $target = $($form.data('target')),
                serviceEndpoint = 'Sitecore8-ServicesClient-Controllers/Product/';
            $.ajax({
                url: '/sitecore/api/ssc/' + serviceEndpoint + id,
                type: 'GET',
                data: $form.serialize() // database,includeMetadata,includeStandardFields
            }).done(function (data, status, hxr) {
                $('<pre>').text(JSON.stringify(data, null, '  ')).appendTo($target);
                $target.bsAlert({
                    type: 'success',
                    message: this.type + ': ' + this.url
                });
            }).fail(function (xhr, textStatus, errorThrown) {
                $target.bsAlert({
                    type: 'danger',
                    title: 'Error',
                    message: errorThrown
                });
            });
            e.preventDefault();
            return false;
        });
    });
})(window, jQuery);