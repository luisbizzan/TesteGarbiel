(function () {
    $.validator.setDefaults({ ignore: null });
    $('.onlyNumber').mask('0#');
    $("dateFormat").mask("99/99/9999");
    $('.hourMinute').mask("99:99", { reverse: true });

    $.validator.addMethod('validateDateOrPrazoInicial', function (value, ele) {
        var dataInicial = $("#Filter_DataInicial").val();

        if (value != "")
            return true
        else if (dataInicial != "")
            return true
        else
            return false;
    }, 'Data Inicial ou Prazo Inicial Obrigatório');

    $.validator.addMethod('validateDateOrPrazoFinal', function (value, ele) {
        var dataFinal = $("#Filter_DataFinal").val();

        if (value != "")
            return true
        else if (dataFinal != "")
            return true
        else
            return false;
    }, 'Data Final ou Prazo Final Obrigatório');

    $.validator.addMethod('validateTime', function (value, ele) {
        if (value === "") {
            return true;
        }
        let regex = /^([0-1]?[0-9]|2[0-4]):([0-5][0-9])(:[0-5][0-9])?$/;
        let validaRegex = regex.test(value);
        if (validaRegex) {
            return true;
        }
        return false;
    }, 'Hora inválida');

    var actionsColumn = dart.dataTables.renderActionsColumn(function (data, type, full, meta) {
        return [
            {
                action: 'details',
                href: view.detailsUrl + '?id=' + full.IdGarantia,
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
            { data: 'IdCliente' },
            { data: 'IdTransportadora' },
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