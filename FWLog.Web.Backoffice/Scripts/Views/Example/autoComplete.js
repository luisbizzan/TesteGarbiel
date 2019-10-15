(function () {

    var $input = $('#AutoCompleteDisplay');

    new dart.AutoComplete($input[0], {
        serviceUrl: view.urlAutoComplete,
        selectedValueInput: $('#AutoCompleteSelectedValue')[0]
    });

    setInterval(function () {
        $('#testvalor').val($('#AutoCompleteSelectedValue').val());
    }, 100);


})();