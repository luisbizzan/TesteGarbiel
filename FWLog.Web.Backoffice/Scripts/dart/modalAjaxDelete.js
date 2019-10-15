// Modal utilizado para a confirmação de uma exclusão.
// Envia uma requisição para o servidor quando o usuário confirma.
(function (dart, resources, undefined) {

    dart.modalAjaxDelete = (function () {

        var modalCloseEvent = function ($modal) {
            $modal.data('bs.modal', null);
            $modal.remove();
        };

        var getHtml = function (title, message, confirmEvent) {

            var container = $('<div/>', { 'class': 'modal fade', 'role': 'dialog' });

            var modal = $('<div/>', { 'class': 'modal-dialog' });
            var content = $('<div/>', { 'class': 'modal-content' });

            var header = $('<div/>', { 'class': 'modal-header' })
                .append($('<button/>', { 'type': 'button', 'class': 'close', 'data-dismiss': 'modal', 'html': '&times;' }))
                .append($('<h4/>', { 'class': 'modal-title', 'text': title }));

            var body = $('<div/>', { 'class': 'modal-body' })
                .append($('<p/>', { 'text': message }));

            var footer = $('<div/>', { 'class': 'modal-footer' })
                .append($('<button/>', {
                    'type': 'button', 'class': 'btn btn-default', 'data-dismiss': 'modal', 'style': 'min-width:125px', 'text': resources.get("CancelAction") }))
                .append($('<button/>', {
                    'type': 'button', 'class': 'btn btn-danger', 'style': 'min-width:125px', 'text': resources.get("DeleteAction"), 'click': confirmEvent }));

            var complete = container.append(
                modal.append(
                    content.append(header, body, footer)
                )
            );

            container.on('hidden.bs.modal', function () {
                modalCloseEvent($(this));
            });

            return complete;
        };

        var postUrl = function (url, onFinish) {
            $.post(url, function (result) {
                if (result.Success) {
                    PNotify.success({ text: result.Message });
                } else {
                    PNotify.error({ text: result.Message });
                }
            }).fail(function () {
                PNotify.error({
                    text: resources.get("AjaxErrorMessage") });                
            })
            .always(function () {
                onFinish();
            });
        };

        // options = { deleteUrl, title (opcional), message (opcional), onConfirm (opcional) }
        var open = function (options) {

            var title = options.title || resources.get("ConfirmTitle");
            var message = options.message || resources.get("DeletionConfirmationMessage");

            var onConfirm = options.onConfirm || function () { };    

            var confirmEvent = function () {
                $modal.modal('hide');
                postUrl(options.deleteUrl, onConfirm);
            };    

            var $modal = getHtml(title, message, confirmEvent).appendTo('body');   

            $modal.modal('show');
        };

        return {
            open: open
        };

    })();

})(window.dart = window.dart || {}, dart.resources);