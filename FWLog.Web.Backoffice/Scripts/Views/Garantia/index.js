﻿(function () {
    $.validator.setDefaults({ ignore: null });
    $('.onlyNumber').mask('0#');
    $("dateFormat").mask("99/99/9999");
    $('.hourMinute').mask("99:99", { reverse: true });

    $.validator.addMethod('validateDateOrPrazoInicial', function (value, ele) {
        var dataInicial = $("#Filter_DataRecebimentoInicial").val();

        if (value !== "")
            return true;
        else if (dataInicial !== "")
            return true;
        else
            return false;
    }, 'Data Emissão Inicial ou Data Recbimento Obrigatório');

    $.validator.addMethod('validateDateOrPrazoFinal', function (value, ele) {
        var dataFinal = $("#Filter_DataRecebimentoFinal").val();

        if (value !== "")
            return true;
        else if (dataFinal !== "")
            return true;
        else
            return false;
    }, 'Data Emissão Final ou Data Recbimento Final Obrigatório');

    var actionsColumn = dart.dataTables.renderActionsColumn(function (data, type, full, meta) {
        return [
            {
                action: 'details',
                href: view.detailsUrl + '?id=' + full.IdGarantia,
                visible: view.detailsVisible
            }
        ];
    });

    var $DataEmissaoInicial = $('#Filter_DataEmissaoInicial').closest('.date');
    var $DataEmissaoFinal = $('#Filter_DataEmissaoFinal').closest('.date');
    var $DataRecebimentoInicial = $('#Filter_DataRecebimentoInicial').closest('.date');
    var $DataRecebimentoFinal = $('#Filter_DataRecebimentoFinal').closest('.date');

    var createLinkedPickers = function () {
        var dataInicial = $DataEmissaoInicial.datetimepicker({
            locale: moment.locale(),
            format: 'L',
            allowInputToggle: true
        });

        var dataFinal = $DataEmissaoFinal.datetimepicker({
            locale: moment.locale(),
            useCurrent: false,
            format: 'L',
            allowInputToggle: true
        });

        new dart.DateTimePickerLink(dataInicial, dataFinal, { ignoreTime: true });
    };

    var createLinkedPickes2 = function () {
        var prazoInicial = $DataRecebimentoInicial.datetimepicker({
            locale: moment.locale(),
            format: 'L',
            allowInputToggle: true
        });

        var prazoFinal = $DataRecebimentoFinal.datetimepicker({
            locale: moment.locale(),
            useCurrent: false,
            format: 'L',
            allowInputToggle: true
        });

        new dart.DateTimePickerLink(prazoInicial, prazoFinal, { ignoreTime: true });
    };

    createLinkedPickers();
    createLinkedPickes2();

    $('#dataTable').DataTable({
        ajax: {
            "url": view.pageDataUrl,
            "type": "POST",
            "data": function (data) {
                dart.dataTables.saveFilterToData(data);
            }
        },
        initComplete: function (settings, json) {
            dart.dataTables.addEventsForDropdownAutoposition($('#dataTable'));
        },
        stateSaveParams: function (settings, data) {
            dart.dataTables.saveFilterToData(data);
        },
        stateLoadParams: function (settings, data) {
            dart.dataTables.loadFilterFromData(data);
        },
        columns: [
            { data: 'IdGarantia' },
            { data: 'Cliente' },
            { data: 'Transportadora' },
            { data: 'NumeroNF' },
            { data: 'NumeroFicticioNF' },
            { data: 'DataEmissao' },            
            { data: 'DataRecebimento' },
            { data: 'GarantiaStatus' },
            actionsColumn
        ]
    });

    dart.dataTables.loadFormFilterEvents();

    $("#pesquisarCliente").click(function () {
        $("#modalCliente").load(HOST_URL + "Cliente/SearchModal", function () {
            $("#modalCliente").modal();
        });
    });

    $("#pesquisarTransportadora").click(function () {
        $("#modalTransportadora").load(HOST_URL + "Transportadora/SearchModal", function () {
            $("#modalTransportadora").modal();
        });
    });

    $("#limparCliente").click(function () {
        limparCliente();
    });

    $("#limparTransportadora").click(function () {
        limparTransportadora();
    });

})();

function setCliente(idCliente, nomeFantasia) {
    $("#Filter_RazaoSocialCliente").val(nomeFantasia);
    $("#Filter_IdCliente").val(idCliente);
    $("#modalCliente").modal("hide");
    $("#modalCliente").empty();
}

function limparCliente() {
    let razaoSocial = $("#Filter_RazaoSocialCliente");
    let cliente = $("#Filter_IdCliente");
    razaoSocial.val("");
    cliente.val("");
}

function setTransportadora(idTransportadora, nomeFantasia) {
    $("#Filter_RazaoSocialTransportadora").val(nomeFantasia);
    $("#Filter_IdTransportadora").val(idTransportadora);
    $("#modalTransportadora").modal("hide");
    $("#modalTransportadora").empty();
}

function limparTransportadora() {
    let razaoSocial = $("#Filter_RazaoSocialTransportadora");
    let cliente = $("#Filter_IdTransportadora");
    razaoSocial.val("");
    cliente.val("");
}