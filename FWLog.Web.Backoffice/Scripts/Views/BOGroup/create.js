(function () {

    $("#checkAllRoles").click(function () {
        $("input[type=checkbox]").prop('checked', true).iCheck('update');
    });

    $("#uncheckAllRoles").click(function () {
        $("input[type=checkbox]").prop('checked', false).iCheck('update');
    });

    $('#roleSearch').on('keyup', function () {
        var search = $(this).val();

        if (!$(this).val()) {
            $('[data-group]').show();
        } else {
            $('[data-group-header]').each(function (e) {
                var $group = $(this).closest('[data-group]');
                if ($(this).text().toUpperCase().indexOf(search.toUpperCase()) >= 0) {
                    $group.show();
                } else {
                    $group.hide();
                }
            });
        }
    });
})();