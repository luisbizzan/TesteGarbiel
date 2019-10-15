(function () {

    $('#checkAllGroups').on('click', function () {
        $('[data-group]').prop('checked', true);
    });

    $('#uncheckAllGroups').on('click', function () {
        $('[data-group]').prop('checked', false);
    });


})();