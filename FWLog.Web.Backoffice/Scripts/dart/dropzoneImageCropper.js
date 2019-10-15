// Adapta um dropzone existente para que apareça uma modal de crop ao selecionar uma imagem.
// Depende de configurações: dart.config.saveTempFileAndCropUrl
(function (dart, resources, undefined) {


    //options = {    
    //    idealWidth: 0,
    //    idealHeight: 0,    
    //}
    dart.DropzoneImageCropper = function (dropzoneInstance, options) {

        options = options || {};
        options.idealWidth = options.idealWidth || 0;
        options.idealHeight = options.idealHeight || 0;
        
        var freeCrop = (options.idealWidth === 0 || options.idealHeight === 0);
        
        var addedImages = [];       

        var hasFileSelected = false;

        var openCropManager = function (src, file) {
            var cropmg = new dart.cropManager({
                idealWidth: options.idealWidth,
                idealHeight: options.idealHeight,
                freeCrop: freeCrop,
                src: src,
                acceptEvent: function (data, getCroppedImageSrc) {
                    var cropArea = {
                        x: data.x,
                        y: data.y,
                        width: data.width,
                        height: data.height
                    };
                    
                    addedImages.push({ file: file, cropArea: cropArea });                    
                    dropzoneInstance.processFile(file);
                    dropzoneInstance.emit("thumbnail", file, getCroppedImageSrc());
                },
                cancelEvent: function () {
                    dropzoneInstance.removeFile(file);
                }
            });

            cropmg.open();
        };

        var addedFileEvent = function (file) {
            
            var reader = new FileReader();

            reader.onload = function (e) {
                var src = e.target.result;
                openCropManager(src, file);
            };

            reader.readAsDataURL(file);
        };

        var getAddedImageByFile = function (file) {
            for (var i = 0; i < addedImages.length; i++) {
                if (addedImages[i].file == file) {
                    return { index: i, image: addedImages[i] };
                }
            }    
            return null;
        };

        var removedFileEvent = function (file) {

            var addedImage = getAddedImageByFile(file);

            if (addedImage != null) {
                addedImages.splice(addedImage.index, 1);
            }            
        };


        var sendingEvent = function (file, xhr, formData) {
            var addedImage = getAddedImageByFile(file).image;            
            formData.append("cropArea.X", parseInt(addedImage.cropArea.x));
            formData.append("cropArea.Y", parseInt(addedImage.cropArea.y));
            formData.append("cropArea.Width", parseInt(addedImage.cropArea.width));
            formData.append("cropArea.Height", parseInt(addedImage.cropArea.height));            
        };

        var load = function () {
            dropzoneInstance.options.url = dart.config.saveTempFileAndCropUrl;
            dropzoneInstance.options.autoProcessQueue = false;
            dropzoneInstance.on('addedfile', addedFileEvent);
            dropzoneInstance.on('removedfile', removedFileEvent);
            dropzoneInstance.on('sending', sendingEvent);
        };

        load();
    };

})(window.dart = window.dart || {}, dart.resources);