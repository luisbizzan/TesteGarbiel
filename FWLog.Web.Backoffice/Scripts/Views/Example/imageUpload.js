(function () {

    var $dropzone = $('.dropzone');
    var dropzone = Dropzone.forElement($dropzone[0]);    

    new dart.DropzoneImageCropper(dropzone, {});

})();
