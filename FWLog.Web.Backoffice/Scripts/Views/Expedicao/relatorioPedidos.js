﻿(function () {
    $.validator.setDefaults({ ignore: null });
    $('.onlyNumber').mask('0#');

    $("dateFormat").mask("99/99/9999");

    $("#pesquisarProduto").click(function () {
        $("#modalProduto").load(HOST_URL + "Produto/SearchModal", function () {
            $("#modalProduto").modal();
        });
    });

    function limparProduto() {
        $("#Filter_DescricaoProduto").val("");
        $("#Filter_IdProduto").val("");
    }

    $("#limparProduto").click(function () {
        limparProduto();
    });

    $.validator.addMethod('validarDataInicial', function (value, ele) {
        var idPedidoVenda = $("#Filter_NumeroPedido").val();
        var dataInicial = $("#Filter_DataInicial").val();
        var dataFinal = $("#Filter_DataFinal").val();

        if (dataInicial == "" && idPedidoVenda == "")
            return false
        else if (dataInicial == "" && idPedidoVenda != "" && dataFinal != "")
            return false
        else
            return true;
    }, 'Informe a data impressão inicial para pesquisar');

    $.validator.addMethod('validarDataFinal', function (value, ele) {
        var idPedidoVenda = $("#Filter_NumeroPedido").val();
        var dataFinal = $("#Filter_DataFinal").val();
        var dataInicial = $("#Filter_DataInicial").val();

        if (dataFinal == "" && idPedidoVenda == "")
            return false
        else if (dataFinal == "" && idPedidoVenda != "" && dataInicial != "")
            return false
        else
            return true;
    }, 'Informe a data impressão final para pesquisar');

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
        var editarVolumeVisivel = view.editarVolumeVisivel && full.PermitirEditarVolume;
        var removerUsuarioSeparacaoVisivel = view.removerUsuarioSeparacaoVisivel && full.PodeRemoverUsuarioSeparacao;

        return [
            {
                text: "Detalhes do Volume",
                attrs: { 'data-id': full.IdPedidoVendaVolume, 'action': 'detailsUrl' },
                icon: 'fa fa-eye',
                visible: view.detailsVisible
            },
            {
                text: "Reimprimir Etiqueta Separação",
                action: 'reimprimir',
                icon: 'fa fa-print',
                attrs: { 'data-id': full.IdPedidoVendaVolume, 'action': 'reimprimir' },
                visible: view.imprimirVisivel
            },
            {
                text: "Editar Volume",
                action: 'editarVolume',
                icon: 'fa fa-edit',
                attrs: { 'data-id': full.IdPedidoVendaVolume, 'action': 'editarVolume' },
                visible: editarVolumeVisivel
            },
            {
                text: "Remover Usuário Separação",
                action: 'confirmaRemocaoUsuarioSeparacao',
                icon: 'fa fa-repeat',
                attrs: { 'data-id': full.IdPedidoVendaVolume, 'data-pedido': full.NroPedido, 'data-volume': full.NroVolume, 'action': 'confirmaRemocaoUsuarioSeparacao' },
                visible: removerUsuarioSeparacaoVisivel
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
        rowCallback: function (row, data, index) {
            $('tr:eq(0)', row).html('<b>' + data[0] + '</b>');
        },
        columns: [
            { "defaultContent": "", width: '8%' },
            { data: 'NroVolume', width: '8%' },
            { data: 'NroCentena', width: '8%' },
            { data: 'DataCriacao' },
            { data: 'DataIntegracao' },
            { data: 'DataImpressao' },
            { data: 'NumeroSerieNotaFiscal' },
            { data: 'DataExpedicao' },
            { data: 'StatusVolume', width: '20%' },
            { data: 'StatusPedido', width: '20%' },
            actionsColumn
        ],
        rowGroup: {
            dataSrc: 'NroPedido',
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

    $("#dataTable").on('click', "[action='reimprimir']", confirmarReimpressaoEtiqueta);
    $("#dataTable").on('click', "[action='editarVolume']", editarVolume);
    $("#dataTable").on('click', "[action='confirmaRemocaoUsuarioSeparacao']", confirmaRemocaoUsuarioSeparacao);

    function confirmarReimpressaoEtiqueta() {
        let id = $(this).data("id");

        let $modal = $("#confirmarReimpressao");

        $modal.load(HOST_URL + CONTROLLER_PATH + "ConfirmarReimpressaoEtiquetaVolume?IdPedidoVendaVolume=" + id, function () {
            $modal.modal();
        });
    }
})();

function editarVolume() {
    let id = $(this).data("id");
    $.ajax({
        url: HOST_URL + CONTROLLER_PATH + "GerenciarVolumesValidarVolume/" + id,
        method: "POST",
        cache: false,
        success: function (result) {
            if (result.Success) {
                window.location.href = HOST_URL + CONTROLLER_PATH + "GerenciarVolumesEditar/?idPedidoVendaVolume=" + id;
            } else {
                PNotify.error({ text: result.Message });
            }
        },
        error: function (data) {
            PNotify.error({ text: "Ocorreu um erro na inicilização da edição do volume." });
            NProgress.done();
        }
    });
}

function setCliente(idCliente, nomeFantasia) {
    $("#Filter_NomeCliente").val(nomeFantasia);
    $("#Filter_IdCliente").val(idCliente);
    $("#modalCliente").modal("hide");
    $("#modalCliente").empty();
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

function imprimir(acao, id) {
    var idImpressora = $("#IdImpressora").val();

    $.ajax({
        url: HOST_URL + CONTROLLER_PATH + acao,
        method: "POST",
        cache: false,
        data: {
            IdImpressora: idImpressora,
            IdPedidoVendaVolume: id
        },
        success: function (result) {
            if (result.Success) {
                PNotify.success({ text: result.Message });

                fechaModalImpressoras();
            } else {
                PNotify.error({ text: result.Message });
            }
        },
        error: function (data) {
            PNotify.error({ text: "Ocorreu um erro na impressão." });
            NProgress.done();
        }
    });
}

function fechaModalImpressoras() {
    var $modal = $("#modalImpressoras");

    $modal.modal("hide");
    $modal.empty();
}

function setProduto(idProduto, descricao) {
    $("#Filter_DescricaoProduto").val(descricao);
    $("#Filter_IdProduto").val(idProduto);
    $("#modalProduto").modal("hide");
    $("#modalProduto").empty();
}

function confirmaRemocaoUsuarioSeparacao() {

    let id = $(this).data("id");
    let pedido = $(this).data("pedido");
    let volume = $(this).data("volume");
    let dataTable = $(this).closest('table');

    dart.modalAjaxConfirm.open({
        title: 'Confirmação de Remoção',
        message: "Deseja realmente remover o usuário da separação do Volume " + volume + " - " + pedido + " ?",
        onConfirm: function () {
            removeUsuarioSeparacao(id, dataTable);
        }
    });
}

function removeUsuarioSeparacao(id, dataTable) {

    $.ajax({
        url: '/Expedicao/RemoverUsuarioSeparacao/' + id,
        type: "POST",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        processData: false,
        contentType: false,
        success: function (result) {

            if (result.Success) {
                PNotify.success({ text: result.Message });

                dataTable.DataTable().ajax.reload(null, false);
            } else {
                PNotify.error({ text: result.Message });
            }
        }
    });
}