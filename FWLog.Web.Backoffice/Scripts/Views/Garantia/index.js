(function () {
    $.validator.setDefaults({ ignore: null });
    $('.onlyNumber').mask('0#');
    $("dateFormat").mask("99/99/9999");
    $('.hourMinute').mask("99:99", { reverse: true });

    $.validator.addMethod('validateDateOrPrazoInicial', function (value, ele) {
        var dataInicial = $("#Filter_Data_Inicial").val();

        if (value !== "")
            return true;
        else if (dataInicial !== "")
            return true;
        else
            return false;
    }, 'Data Inicial');

    $.validator.addMethod('validateDateOrPrazoFinal', function (value, ele) {
        var dataFinal = $("#Filter_Data_Final").val();

        if (value !== "")
            return true;
        else if (dataFinal !== "")
            return true;
        else
            return false;
    }, 'Data Final Obrigatório');

    var actionsColumn = dart.dataTables.renderActionsColumn(function (data, type, full, meta) {
        var visivelEstornar = view.estornarSolicitacao;
        var visivelConferir = view.conferirSolicitacao && (full.Id_Status === 22 || full.Id_Status === 23);

        return [
            {
                text: "Visualizar",
                attrs: { 'data-id': full.Id, 'action': 'visualizarSolicitacao' },
                icon: 'fa fa-eye',
                visible: view.detailsVisible
            },
            {
                text: "Estornar",
                attrs: { 'data-id': full.Id, 'action': 'estornarSolicitacao' },
                icon: 'fa fa-pencil-square',
                visible: visivelEstornar
            },
            {
                text: "Conferir",
                attrs: { 'data-id': full.Id, 'action': 'conferirSolicitacao' },
                icon: 'fa fa-check-square-o',
                visible: visivelConferir
            }
        ];
    });

    $("#dataTable").on('click', "[action='visualizarSolicitacao']", visualizarSolicitacao);
    //$("#dataTable").on('click', "[action='estornarSolicitacao']", registrarRecebimento);
    //$("#dataTable").on('click', "[action='conferirSolicitacao']", conferirSolicitacao);

    var $Data_Inicial = $('#Filter_Data_Inicial').closest('.date');
    var $Data_Final = $('#Filter_Data_Final').closest('.date');

    var createLinkedPickers = function () {
        var dataInicial = $Data_Inicial.datetimepicker({
            locale: moment.locale(),
            format: 'L',
            allowInputToggle: true
        });

        var dataFinal = $Data_Final.datetimepicker({
            locale: moment.locale(),
            useCurrent: false,
            format: 'L',
            allowInputToggle: true
        });

        new dart.DateTimePickerLink(dataInicial, dataFinal, { ignoreTime: true });
    };

    createLinkedPickers();

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
            { data: 'Cli_Cnpj', },
            { data: 'Id', },
            { data: 'Dt_Criacao', },
            { data: 'Tipo', },
            { data: 'Valor', },
            { data: 'Status', },
            actionsColumn
        ]
    });

    dart.dataTables.loadFormFilterEvents();
})();

function visualizarSolicitacao() {
    var id = $(this).data("id");
    let modal = $("#modalVisualizar");

    modal.load(CONTROLLER_PATH + "VisualizarSolicitacao/" + id, function () {
        modal.modal();
    });
}

function conferirSolicitacao() {
    let id = $(this).data("id");
    let $modal = $("#modalConferenciaGarantia");

    $.ajax({
        url: HOST_URL + CONTROLLER_PATH + "ValidarInicioConferenciaDaGarantia/" + id,
        cache: false,
        method: "POST",
        success: function (result) {
            if (result.Success) {
                $modal.load(HOST_URL + CONTROLLER_PATH + "EntradaConferenciaGarantia/" + id, function (result) {
                    $modal.modal();

                    $("#Referencia").focus();
                });
                if (result.Message != "")
                    PNotify.warning({ text: result.Message });
            } else {
                PNotify.warning({ text: result.Message });
            }
        },
        error: function (request, status, error) {
            if (request.responseText == 'undefined') {
                PNotify.error({ text: 'Um erro inesperado ocorreu, atualize a página e tente novamente.' });
            }
            else {
                PNotify.error({ text: request.responseText });
            }
        }
    });
}