// Modal utilizado para a confirmação de uma ação.
// Envia a requisição especificada para o servidor quando o usuário confirma.
(function (dart, resources, undefined) {

    dart.modalAjaxConfirm = (function () {

        var modalCloseEvent = function ($modal) {
            $modal.data('bs.modal', null);
            $modal.remove();
        };

        var getHtml = function (title, message, confirmEvent, confirmText, cancelText, cancelEvent) {

            var container = $('<div/>', { 'class': 'modal fade', 'role': 'dialog' });

            var modal = $('<div/>', { 'class': 'modal-dialog' });
            var content = $('<div/>', { 'class': 'modal-content' });

            var header = $('<div/>', { 'class': 'modal-header' })
                .append($('<button/>', { 'type': 'button', 'class': 'close', 'data-dismiss': 'modal', 'html': '&times;' }))
                .append($('<h4/>', { 'class': 'modal-title', 'text': title }));

            var body = $('<div/>', { 'class': 'modal-body' })
                .append($('<p/>', { 'text': message }));

            var footer = $('<div/>', { 'class': 'modal-footer' })
                .append($('<button/>', { 'type': 'button', 'class': 'btn btn-default', 'data-dismiss': 'modal', 'style': 'min-width:125px', 'text': cancelText, 'click': cancelEvent }))
                .append($('<button/>', { 'type': 'button', 'class': 'btn btn-primary', 'style': 'min-width:125px', 'text': confirmText, 'click': confirmEvent }));

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
                    text: resources.get("AjaxErrorMessage")
                });
            }).always(function (result) {
                onFinish(result);
            });
        };

        // options = { url, title (opcional), message (opcional), onConfirm (opcional), confirmText (opcional), cancelText(opcional) }
        var open = function (options) {

            var title = options.title || resources.get("ConfirmTitle");
            var message = options.message || resources.get("ActionConfirmationMessage");
            var onConfirm = options.onConfirm || function () { };
            var onCancel = options.onCancel || function () { };
            var confirmText = options.confirmText || resources.get("ConfirmAction");
            var cancelText = options.cancelText || resources.get("CancelAction");

            var confirmEvent = function () {
                $modal.modal('hide');

                if (options.url) {
                    postUrl(options.url, onConfirm);
                } else {
                    onConfirm();
                }
            };

            var cancelEvent = function () {
                onCancel();
                $modal.modal('hide');
            };

            var $modal = getHtml(title, message, confirmEvent, confirmText, cancelText, cancelEvent).appendTo('body');

            $modal.modal('show');
        };

        return {
            open: open
        };

    })();

})(window.dart = window.dart || {}, dart.resources);