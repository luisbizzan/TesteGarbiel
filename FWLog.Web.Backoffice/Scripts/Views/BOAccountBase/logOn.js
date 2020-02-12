(function () {

    

    $('[data-language]').on('click', function () {
        debugger
        var selected = $(this).attr('data-language');
        window.location = getLanguageUrl(selected);
    });

    var getLanguageUrl = function (culture) {
        debugger
        return view.logOnUrl + "?l=" + encodeURIComponent(culture);
    };

    $(".form-control").on('keydown', document_Keydown);

    function document_Keydown(e) {
        
        if (e.which == 13) {
            $('#confirm-btn').click();
            return false;
        }
    }

})();