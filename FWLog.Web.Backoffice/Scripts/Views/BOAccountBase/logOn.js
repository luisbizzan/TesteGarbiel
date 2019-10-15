(function () {

    $('[data-language]').on('click', function () {        
        var selected = $(this).attr('data-language');
        window.location = getLanguageUrl(selected);
    });

    var getLanguageUrl = function (culture) {
        return view.logOnUrl + "?l=" + encodeURIComponent(culture);
    };

})();