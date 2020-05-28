(function () {
    $.validator.setDefaults({ ignore: null });
    $('.onlyNumber').mask('0#');

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

    var actionsColumn = dart.dataTables.renderActionsColumn(function (data, type, full, meta) {
        return [
            {
                text: "Detalhes do Volume",
                attrs: { 'data-id': full.IdPedidoVendaVolume, 'action': 'detalhesPedidoVolume' },
                icon: 'fa fa-eye',
                visible: view.detailsVisible
            }
        ];
    });

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
            { "defaultContent": "", width: '10%' },
            { data: 'NroVolume', width: '10%' },
            { data: 'NomeTransportadora', width: '15%' },
            { data: 'DataDoPedido' },
            { data: 'DataSaidaDoPedido' },
            { data: 'Status' },
            actionsColumn
        ],
        order: [[2, 'asc']],
        rowGroup: {
            dataSrc: 'NroPedido'
        }
    });

    $('#dataTable').dataTable.error = function (settings, helpPage, message) {
        console.log(message)
    };

    $("#dataTable").on('click', "[action='detailsUrl']", detalhesPedido);

    dart.dataTables.loadFormFilterEvents();

    createLinkedPickers();

    $(".clearButton").click(function () {
        $('#Filter_DataInicial').val("");
        $('#Filter_DataFinal').val("");
    });

    $("#pesquisarPedidoVenda").click(function () {
        limpaModais();

        $("#modalPesquisaPedidoVenda").load(HOST_URL + "Expedicao/PedidoVendaPesquisaModal/", function () {
            $("#modalPesquisaPedidoVenda").modal();
        });
    });

    $("#limparPedidoVenda").click(function () {
        $("#Filter_NumeroPedidoVenda").val("");
        $("#Filter_IdPedidoVenda").val("");
    });

})();

function selecionarPedidoVenda(idPedidoVenda, numeroPedidoVenda) {
    $("#Filter_NumeroPedidoVenda").val(numeroPedidoVenda);
    $("#Filter_IdPedidoVenda").val(idPedidoVenda);
    $("#modalPesquisaPedidoVenda").modal("hide");
    $("#modalPesquisaPedidoVenda").empty();
}

function detalhesPedido() {
    var id = $(this).data("id");
    let modalDetalhesPedidoVolume = $("#modalDetalhesPedidoVolume");
    alert(id);
    modalDetalhesPedidoVolume.load(CONTROLLER_PATH + "DetalhesPedidoVolume/" + id, function () {
        modalDetalhesPedidoVolume.modal();
    });
}