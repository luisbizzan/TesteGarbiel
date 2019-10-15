// Utilidades para o componente dropzone (download/upload de imagem).
// Depende de configurações: dart.config.saveTempFileUrl, dart.config.downloadFileUrl
(function (dart, resources, undefined) {

    Dropzone.autoDiscover = false;

    $('.dropzone').each(function () {
        var form = $(this).closest('form');
        var inputName = $(this).attr('data-input-name');
        var maxFile = $(this).attr('data-max-file');
        var required = ($(this).attr('data-required') == 'true');
        var requiredGuid = createGuid();

        if ($(this).attr('data-scroll') === 'true') {
            $(this).addClass('dropzone-hscroll');
        }

        if (required) {
            var requiredInput = $('<input class="drop-required" type="hidden" required data-val="true" data-val-required="' + resources.get("DropzoneRequired") + '" value="" />');
            requiredInput.addClass('can-validate').addClass('form-control').attr('name', requiredGuid);
            var span = $('<span class="field-validation-valid" data-valmsg-for="' + requiredGuid + '" data-valmsg-replace="true" />')
            $(this).parent().append(requiredInput);
            $(this).parent().append(span);
        }

        var myDropzone = new Dropzone(this, {
            maxFiles: $(this).attr('data-max-file'),
            addRemoveLinks: $(this).attr('data-readonly') === 'true' ? false : true,
            paramName: 'file',
            dictDefaultMessage: $(this).attr('data-readonly') === 'true' ? resources.get("DropzoneNoFilesMessage") : resources.get("DropzoneSelectFileMessage"),
            dictRemoveFile: resources.get("DropzoneRemoveFileAction"),
            dictMaxFilesExceeded: 'Test',
            dictInvalidFileType: 'Test File',
            url: $(this).attr('data-url') ? $(this).attr('data-url') : dart.config.saveTempFileUrl,
            acceptedFiles: $(this).attr('data-accepted-files') ? $(this).attr('data-accepted-files') : '',
            clickable: $(this).attr('data-readonly') === 'true' ? false : true,
            init: function () {
                var myDropzone = this;
                var configUniqueName = $(myDropzone.element).attr('data-config-unique-name');
                var inputCount = 0;

                myDropzone.on('sending', function (file, xhr, formData) {
                    formData.append('configUniqueName', configUniqueName);
                });

                myDropzone.on('success', function (file, response) {
                    successFunction(response.FileName, form, inputName, maxFile, myDropzone.element, file.previewElement);

                    if (required) {
                        $(form).find('input.drop-required').val('required').valid();
                    }
                });

                myDropzone.on('error', function (file) {
                    myDropzone.removeFile(file);

                    // Check error caused by file type invalid
                    if (!Dropzone.isValidFile(file, this.options.acceptedFiles)) {
                        var message = resources.get("FileTypeInvalid");
                        dart.dropzone.showError(message);
                    }
                });

                myDropzone.on('maxfilesexceeded', function (file) {
                    var message = resources.get("MaxFilesExceeded");
                    dart.dropzone.showError(message);
                });

                myDropzone.on('removedfile', function (file) {
                    var fileName = $(file.previewElement).attr('data-file-name');
                    var input = $(form).find('input[value="' + fileName + '"]');
                    $(input).remove();
                    recalculateArraySize(form, inputName, myDropzone.element, maxFile);

                    if (required) {
                        var countFiles = $(myDropzone.element).children('.dz-preview');
                        if (countFiles.length == 0) {
                            $(form).find('input.drop-required').val('').valid();
                        }
                    }
                });
            }
        });

        $(this).find('.dropzone-load-file').each(function () {
            var mockFile = {
                name: $(this).attr('data-name'),
                size: 12345,
                path: $(this).attr('data-path'),
                status: Dropzone.ADDED,
                accepted: true
            };

            if (mockFile.name != undefined && mockFile.name && mockFile.name != '') {
                myDropzone.files.push(mockFile);

                myDropzone.options.addedfile.call(myDropzone, mockFile);

                myDropzone.options.thumbnail.call(myDropzone, mockFile, mockFile.path);

                myDropzone.options.success.call(myDropzone, mockFile);

                myDropzone.options.complete.call(myDropzone, mockFile);

                mockFile.previewElement.classList.add('dz-success');
                mockFile.previewElement.classList.add('dz-complete');

                successFunction(mockFile.name, form, inputName, maxFile, myDropzone.element, mockFile.previewElement);

                var params = {
                    fileName: mockFile.name,
                    configUniqueName: $(this).attr('data-config-unique-name')
                };

                var elementDownload = document.createElement('a');
                var onClick = document.createAttribute('onclick');
                onClick.value = 'dart.dropzone.downloadFile(' + JSON.stringify(params) + ')';
                elementDownload.setAttributeNode(onClick);
                elementDownload.innerHTML = '<i class="fa fa-arrow-circle-down fa-4x" title="' + resources.get("DownloadFileAction") + '"></i>';

                $(myDropzone.element).find('.dz-filename').addClass('dz-download-mark').html(elementDownload);
                $(myDropzone.element).find('.dz-size').html('');
            }
        });
    });

    dart.dropzone = (function () {
        //params = {
        //    fileName: string,
        //    configUniqueName: string
        //}
        function downloadFile(params) {
            window.open(dart.config.downloadFileUrl + '?fileName=' + params.fileName + '&configUniqueName=' + params.configUniqueName, '_blank');
        }

        function showError(message) {
            new PNotify({
                title: resources.get("ErrorTitle"),
                text: message,
                type: 'error',
                hide: "true"
            });
        }

        return {
            downloadFile: downloadFile,
            showError: showError
        };
    })();

    function successFunction(fileName, form, inputName, maxFile, dropzoneElement, thumbnailElement) {
        var newName = inputName;

        if (parseInt(maxFile) > 1) {
            newName += '[0]';
        }

        var input = $('<input />').attr('name', newName).attr('type', 'hidden');
        form.append(input);

        input.val(fileName);
        $(dropzoneElement).find('.dz-filename').html('');
        $(thumbnailElement).attr('data-file-name', fileName);
        recalculateArraySize(form, inputName, dropzoneElement, maxFile);
    }

    function recalculateArraySize(form, inputName, dropzoneElement, maxFile) {
        if (parseInt(maxFile) > 1) {
            var count = $(dropzoneElement).children('.dz-preview');
            var inputs = $(form).find('input[name^="' + inputName + '"]');

            if (count.length == inputs.length) {
                for (var i = 0; i < count.length; i++) {
                    var name = $(inputs[i]).attr('name');
                    var newName = name.replace(/\[\d](?!.*\[)/, '[' + i + ']');
                    $(inputs[i]).attr('name', newName);
                }
            }
        }
    }

    function createGuid() {
        var array = new Uint32Array(1);
        window.crypto.getRandomValues(array);
        return array[0];
    }

})(window.dart = window.dart || {}, dart.resources);