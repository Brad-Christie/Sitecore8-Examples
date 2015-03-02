(function (window, $, undefined) {
    'use strict';

    $(function () {

        function isID(id){
            if (!id) return false;
            if (id.length != 36 && id.length != 38) return false;
            return id.match(/\{?[a-f0-9]{8}(?:\-[a-f0-9]{4}){3}\-[a-f0-9]{12}\}?/i);
        }
        function isPath(path){
            if (!path) return false;
            return path.match(/^\/?\w[\s\w]*(?:\/[\s\w]+)+/);
        }

        function getDatabase() {
            var db = $('#default-database').val();
            return db && db.length > 0 ? 'database=' + encodeURIComponent(db) : '';
        }

        // GET
        $('#get-form').on('submit', function (e) {
            var $form = $(this),
                $target = $($form.data('target')),
                id = $('#get-id').val();

            if (isID(id)) {
                id = encodeURIComponent(id);
            } else {
                $target.bsAlert({
                    type: 'danger',
                    title: 'Invalid Field',
                    message: 'The Item ID must be a valid Sitecore ID.'
                });
                e.preventDefault();
                return false;
            }

            $.ajax({
                url: '/sitecore/api/ssc/item/' + id,
                type: 'GET',
                data: $form.serialize() + '&' + getDatabase()
            }).done(function (data, status, xhr) {
                $target.append($('<pre>').text(JSON.stringify(data, null, '  ')));
                $target.bsAlert({
                    type: 'success',
                    message: this.type + ': ' + this.url
                });
            }).fail(function (xhr, textStatus, errorThrown) {
                if (xhr.status == 404) {
                    $target.bsAlert({
                        type: 'danger',
                        title: 'Error',
                        message: textStatus + '; Make sure you supplied a valid id.'
                    });
                } else {
                    $target.bsAlert({
                        type: 'danger',
                        title: 'Error',
                        message: errorThrown
                    });
                }
            });
            e.preventDefault();
            return false;
        });

        // POST
        $('#post-form').on('submit', function (e) {
            var $form = $(this),
                $target = $($form.data('target')),
                path = $('#post-path').val(),
                data = {
                    TemplateID: $('#post-templateid').val(),
                    ItemName: $('#post-itemname').val(),
                    Title: $('#post-title').val(),
                    Text: $('#post-text').val()
                };

            if (isPath(path)) {
                path = encodeURIComponent(path.substr(1));
            } else {
                $target.bsAlert({
                    type: 'danger',
                    title: 'Invalid Field',
                    message: 'The Destination Path must be a valid Sitecore path.'
                });
                e.preventDefault();
                return false;
            }

            $form.find(':input[name]').each(function () {
                var $el = $(this);
                data[$this.prop('name')] = $this.val();
            });
            $.ajax({
                url: '/sitecore/api/ssc/item/' + path + '?' + getDatabase(),
                type: 'POST',
                data: JSON.stringify(data),
                contentType: 'application/json'
            }).done(function (data, status, xhr) {
                if (xhr.status == 201) {
                    $target.bsAlert({
                        type: 'info',
                        message: 'Item created at <u>' + xhr.getResponseHeader('Location') + '</u>.'
                    })
                } else {
                    $target.append($('<pre>').text(JSON.stringify(data, null, '  ')));
                }                
                $target.bsAlert({
                    type: 'success',
                    message: this.type + ': ' + this.url
                });
            }).fail(function (xhr, textStatus, errorThrown) {
                $target.bsAlert({
                    type: 'danger',
                    title: 'Error',
                    message: errorThrown + '; Make sure you are logged into sitecore first.'
                });
            });
            e.preventDefault();
            return false;
        });

        // PATCH
        $('#patch-form-prep').on('submit', function (e) {
            var $form = $(this),
                $target = $($form.data('target')),
                $putForm = $('#patch-form'),
                $deleteForm = $('#delete-form');
            // using the ?path= method of item lookup here (instead of by an ID
            // like i did in the GET example)
            $.ajax({
                url: '/sitecore/api/ssc/item/',
                type: 'GET',
                data: $form.serialize() + '&' + getDatabase()
            }).done(function (data) {
                $('#patch-id').val(data.ItemID);
                $('#patch-itemname').val(data.ItemName);
                $('#patch-templateid').val(data.TemplateID);
                $('#patch-title').val(data.Title);
                $('#patch-text').val(data.Text);
                $target.bsAlert({
                    type: 'success',
                    title: 'Success',
                    message: 'Please make some modifications to the item below and click <span class="label label-default">Execute</span>.'
                });
                $target.bsAlert({
                    type: 'success',
                    message: this.type + ': ' + this.url
                });

                $putForm.find('button[type="submit"]').prop('disabled', false);

                $('#delete-id').val(data.ItemID);
                $deleteForm.find('button[type="submit"]').prop('disabled', false);
            }).fail(function (xhr, textStatus, errorThrown) {
                if (xhr.status == 404) {
                    $target.bsAlert({
                        type: 'danger',
                        title: 'Not Found',
                        message: 'Unable to locate item; have you created or published the item yet?'
                    });
                } else {
                    $target.bsAlert({
                        type: 'danger',
                        title: 'Error',
                        message: errorThrown
                    });
                }
                $putForm.find('button[type="submit"]').prop('disabled', true);
                $deleteForm.find('button[type="submit"]').prop('disabled', true);
            });
            e.preventDefault();
            return false;
        });
        $('#patch-form').on('submit', function (e) {
            var $form = $(this),
                $target = $($form.data('target')),
                id = $('#patch-id').val(),
                data = {
                    TemplateID: $('#patch-templateid').val(),
                    ItemName: $('#patch-itemname').val(),
                    Title: $('#patch-title').val(),
                    Text: $('#patch-text').val()
                };

            if (isID(id)) {
                id = encodeURIComponent(id);
            } else {
                $target.bsAlert({
                    type: 'danger',
                    title: 'Invalid Field',
                    message: 'The Item ID must be a valid Sitecore ID.'
                });
                e.preventDefault();
                return false;
            }

            $.ajax({
                url: '/sitecore/api/ssc/item/' + id + '?' + getDatabase(),
                type: 'PATCH',
                data: JSON.stringify(data),
                contentType: 'application/json'
            }).done(function (data, status, xhr) {
                $target.bsAlert({
                    type: 'success',
                    message: this.type + ': ' + this.url
                });
                $target.bsAlert({
                    type: 'success',
                    message: this.type + ': ' + this.url
                });
            }).fail(function (xhr, textStatus, errorThrown) {
                if (xhr.status == 404) {
                    $target.bsAlert({
                        type: 'danger',
                        title: 'Not Found',
                        message: 'Unable to locate item; If the item is not published you cannot update it.'
                    });
                } else {
                    $target.bsAlert({
                        type: 'danger',
                        title: 'Error',
                        message: errorThrown + '; Make sure you are logged into sitecore first.'
                    });
                }
            });
            e.preventDefault();
            return false;
        });

        // DELETE
        $('#delete-form').on('submit', function (e) {
            var $form = $(this),
                $target = $($form.data('target')),
                id = $('#delete-id').val();

            if (isID(id)) {
                id = encodeURIComponent(id);
            } else {
                $target.bsAlert({
                    type: 'danger',
                    title: 'Invalid Field',
                    message: 'The Item ID must be a valid Sitecore ID.'
                });
                e.preventDefault();
                return false;
            }

            $.ajax({
                url: '/sitecore/api/ssc/item/' + id + '?' + getDatabase(),
                type: 'DELETE'
            }).done(function (data, status, xhr) {
                $target.bsAlert({
                    type: 'success',
                    message: this.type + ': ' + this.url
                });
                $target.bsAlert({
                    type: 'success',
                    message: this.type + ': ' + this.url
                });
            }).fail(function (xhr, textStats, errorThrown) {
                $target.bsAlert({
                    type: 'danger',
                    title: 'Error',
                    message: errorThrown + '; Make sure you are logged into sitecore first.'
                });
            });
            e.preventDefault();
            return false;
        });
    });
})(window, jQuery);