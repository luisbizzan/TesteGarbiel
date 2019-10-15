// Utilidade que abre uma modal para crop de imagens.
(function (dart, resources, undefined) {

    //options = {
    //    idealWidth: 0,
    //    idealHeight: 0,
    //    acceptEvent: function (data) {},
    //    cancelEvent: function (data) {},
    //    src: "",
    //    freeCrop: false
    //}
    dart.cropManager = function (options) {

        options.freeCrop = options.freeCrop || false;
        
        var lastOpenAccepted = false;

        var modalHtml =
            '<div class="modal fade drt-modal-crop" role="dialog">' +
            '<div class="modal-dialog">' +
            '<div class="modal-content">' +
            '<div class="modal-header">' +
            '<button type="button" class="close" data-dismiss="modal">&times;</button>' +
            '<h4 class="modal-title">' + resources.get('SelectAnAreaTitle') + '</h4>' +
            '</div>' +
            '<div class="modal-body">' +
            '<div class="img-container"><img data-crop-manager="img" src="" /></div>' +
            '</div>' +
            '<div class="modal-footer">' +
            '<button type="button" class="btn btn-primary" data-crop-manager="confirm">' + resources.get('ConfirmAction') + '</button>' +
            '<button type="button" class="btn btn-default" data-dismiss="modal">' + resources.get('CancelAction') + '</button>' +
            '</div>' +
            '</div>' +
            '</div>' +
            '</div>';

        var $modal = $(modalHtml).appendTo('body');

        var $elements = {
            modal: $modal,
            img: $modal.find('[data-crop-manager="img"]'),
            accept: $modal.find('[data-crop-manager="confirm"]')
        };

        var open = function () {

            lastOpenAccepted = false;

            $elements.img.cropper('destroy');
            $elements.img.attr('src', options.src);
            $elements.img.hide();
            $elements.accept.off();

            var cropperOptions = {
                aspectRatio: options.freeCrop ? NaN : (options.idealWidth / options.idealHeight),
                minCropBoxWidth: options.freeCrop ? 0 : options.idealWidth,
                minCropBoxHeight: options.freeCrop ? 0 : options.idealHeight
            };

            $elements.modal.on('shown.bs.modal', function () {                
                $elements.img.cropper({
                    viewMode: 1,
                    aspectRatio: cropperOptions.aspectRatio,
                    minCropBoxWidth: cropperOptions.minCropBoxWidth,
                    minCropBoxHeight: cropperOptions.minCropBoxHeight,
                    movable: false,
                    zoomable: false,
                    rotatable: false,
                    scalable: false,
                    crop: function (e) {

                    },
                    ready: function () {
                        
                        $elements.accept.on('click', function () {;
                            var data = $elements.img.cropper('getData');
                            $elements.modal.modal('hide');
                            lastOpenAccepted = true;
                            options.acceptEvent(data, function () {
                                return $elements.img.cropper('getCroppedCanvas').toDataURL();
                            });
                        });

                        if (options.freeCrop) {
                            $elements.img.cropper('setData', { x: 0, y: 0, width: 9999, height: 9999 });
                        }
                    }
                });
            });

            $elements.modal.modal('show');
        };

        var cancel = function () {
            $elements.img.cropper('destroy');
            options.cancelEvent();
        };

        var load = function () {
            $(document).ready(function () {
                $elements.modal.on('hidden.bs.modal', function () {
                    // Independente de ter confirmado a edição ou não, o evento de modal fechado é chamado.
                    // Portanto o cancelamento só deve ser chamado se a edição não foi aceita.
                    if (!lastOpenAccepted) {
                        cancel();
                    }
                });
            });
        };

        load();

        return {
            open: open
        }

    };

})(window.dart = window.dart || {}, dart.resources);