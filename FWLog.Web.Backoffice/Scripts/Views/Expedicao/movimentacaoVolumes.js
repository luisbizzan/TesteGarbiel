(function () {
    $.validator.setDefaults({ ignore: [] });

    $("dateFormat").mask("99/99/9999");

    var $DataInicial = $('#Filter_DataInicial').closest('.date');
    var $DataFinal = $('#Filter_DataFinal').closest('.date');

    var createLinkedPickers = function () {
        var dataInicial = $DataInicial.datetimepicker({
            locale: moment.locale(),
            format: 'L',
            allowInputToggle: true
        });

        var dataFinal = $DataFinal.datetimepicker({
            locale: moment.locale(),
            useCurrent: false,
            format: 'L',
            allowInputToggle: true
        });

        new dart.DateTimePickerLink(dataInicial, dataFinal, { ignoreTime: true });
    };

    createLinkedPickers();

    $(".clearButton").click(function () {
        $('.form-control').val("");
        $('.form-control').empty();

        $("#submitButton").click();
    });

    $(".abrirDetalhes").click(function (e) {

        e.preventDefault();

        $("#modalDetalhes").empty();

        var link = $(this).attr("href");

        $("#modalDetalhes").load(link, function () {

            $("#modalDetalhes").modal();
        });
    });
})();