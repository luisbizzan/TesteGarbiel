// Extensão do autocomplete já existente da devbridge.
(function (dart, resources, undefined) {

    //input = input para qual será criado o autocomplete
    //options = {
    //    selectedValueInput(opcional): elemento para qual o valor selecionado deve ser preenchido    
    //}
    // também é aceita todas as opções do autocomplete da devbridge
    dart.AutoComplete = function (input, options) {

        var selectedValueInput = options.selectedValueInput || null;

        var emptyFn = function () { };

        var onSelect = options.onSelect || emptyFn;

        options.onSelect = function (suggestion) {

            if (selectedValueInput !== null) {
                $(selectedValueInput).val(suggestion.data).valid();
            }

            onSelect(suggestion);
        };

        $(input).autocomplete(options);

        if (selectedValueInput) {
            $(input).on('keyup', function () {
                if ($(this).val().length == 0) {
                    $(selectedValueInput).val('').valid();
                }
            });
        }

    };


})(window.dart = window.dart || {}, dart.resources);