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

    $('#dataTable').DataTable({
        ajax: {
            "url": view.pageDataUrl,
            "type": "POST",
            "data": function (data) {
                dart.dataTables.saveFilterToData(data);
            },
            "error": function (data) {
                if (!!(data.statusText)) {
                    NProgress.done();
                }
            }
        },
        deferLoading: 0,
        initComplete: function (settings, json) {
            dart.dataTables.addEventsForDropdownAutoposition($('#dataTable'));
        },
        stateSaveParams: function (settings, data) {
            dart.dataTables.saveFilterToData(data);
        },
        columns: [
            { data: 'NroPedido', },
            { data: 'NroVolume', },
            { data: 'NroCentena', },
            { data: 'DataDoPedido', },
            { data: 'IdENomeTransportadora', },
            { data: 'NotaFiscalESerie', },
            { data: 'DataSaidaDoPedido', },
        ]
    });

    $('#dataTable').dataTable.error = function (settings, helpPage, message) {
        console.log(message)
    };

    dart.dataTables.loadFormFilterEvents();

    createLinkedPickers();

    $(".clearButton").click(function () {
        $('#Filter_DataInicial').val("");
        $('#Filter_DataFinal').val("");
    });

    $("#pesquisarTransportadora").click(function () {
        $("#modalTransportadora").load(HOST_URL + "Transportadora/SearchModal", function () {
            $("#modalTransportadora").modal();
        });
    });

    $("#limparTransportadora").click(function () {
        limparTransportadora();
    });
})();

function limparTransportadora() {
    let razaoSocial = $("#Filter_NomeTransportadora");
    let cliente = $("#Filter_IdTransportadora");
    razaoSocial.val("");
    cliente.val("");
}

function setTransportadora(idTransportadora, nomeFantasia) {
    $("#Filter_NomeTransportadora").val(nomeFantasia);
    $("#Filter_IdTransportadora").val(idTransportadora);
    $("#modalTransportadora").modal("hide");
    $("#modalTransportadora").empty();
}
