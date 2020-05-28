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
                attrs: { 'data-id': full.IdPedidoVendaVolume, 'action': 'detailsUrl' },
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
        ordering: false,
        deferLoading: 0,
        initComplete: function (settings, json) {
            dart.dataTables.addEventsForDropdownAutoposition($('#dataTable'));
        },
        stateSaveParams: function (settings, data) {
            dart.dataTables.saveFilterToData(data);
        },
        columns: [
            { "defaultContent": "", width: '8%' },
            { data: 'NroVolume', width: '8%' },
            { data: 'DataCriacao' },
            { data: 'DataSaida' },
            { data: 'Status', width: '20%' },
            actionsColumn
        ],
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

    $("#pesquisarCliente").click(function () {
        $("#modalCliente").load(HOST_URL + "Cliente/SearchModal", function () {
            $("#modalCliente").modal();
        });
    });

    $("#limparCliente").click(function () {
        limparCliente();
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

function setCliente(idCliente, nomeFantasia) {
    $("#Filter_NomeCliente").val(nomeFantasia);
    $("#Filter_IdCliente").val(idCliente);
    $("#modalCliente").modal("hide");
    $("#modalCliente").empty();
}

function selecionarPedidoVenda(idPedidoVenda, numeroPedidoVenda) {
    $("#Filter_NumeroPedidoVenda").val(numeroPedidoVenda);
    $("#Filter_IdPedidoVenda").val(idPedidoVenda);
    $("#modalPesquisaPedidoVenda").modal("hide");
    $("#modalPesquisaPedidoVenda").empty();
}

function detalhesPedido() {
    var id = $(this).data("id");
    let modalDetalhesPedidoVolume = $("#modalDetalhesPedidoVolume");
    modalDetalhesPedidoVolume.load("DetalhesPedidoVolume/" + id, function () {
        modalDetalhesPedidoVolume.modal();
    });
}

function limparCliente() {
    let razaoSocial = $("#Filter_NomeCliente");
    let cliente = $("#Filter_IdCliente");
    razaoSocial.val("");
    cliente.val("");
}

function limpaModais() {
    $("#modalPesquisaTransportadora").empty();
    $("#modalPesquisaTransportadoraEndereco").empty();
    $("#modalPesquisaPedidoVenda").empty();
}

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