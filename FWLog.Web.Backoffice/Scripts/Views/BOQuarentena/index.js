(function () {
    $('.onlyNumber').mask('0#');
    var actionsColumn = dart.dataTables.renderActionsColumn(function (data, type, full, meta) {
        return [
            {
                text: "Atualizar Status da Quarentena",
                attrs: { 'data-id': full.IdQuarentena, 'action': 'alterarStatus' },
                icon: 'fa fa-pencil-square',
                visible: view.registrarRecebimento
            },
            {
                text: "Emitir Termo de Responsabilidade",
                attrs: { 'data-id': full.IdQuarentena, 'action': 'termoResponsabilidade' },
                icon: 'fa fa-file-text',
                visible: (full.IdQuarentenaStatus === 1 || full.IdQuarentenaStatus === 2) && full.LoteStatus !== "FinalizadoDevolucaoTotal" ? true : false
            },
            {
                text: "Histórico da Quarentena",
                attrs: { 'data-id': full.IdQuarentena, 'action': 'historicoQuarentena' },
                icon: 'fa fa-history',
                visible: view.registrarRecebimento
            }
        ];
    });

    var $DataAberturaInicial = $('#Filter_DataAberturaInicial').closest('.date');
    var $DataAberturaFinal = $('#Filter_DataAberturaFinal').closest('.date');
    var $DataEncerramentoInicial = $('#Filter_DataEncerramentoInicial').closest('.date');
    var $DataEncerramentoFinal = $('#Filter_DataEncerramentoFinal').closest('.date');

    var createLinkedPickers = function () {
        var dataAberturaInicial = $DataAberturaInicial.datetimepicker({
            locale: moment.locale(),
            format: 'L',
            allowInputToggle: true
        });

        var dataAberturaFinal = $DataAberturaFinal.datetimepicker({
            locale: moment.locale(),
            useCurrent: false,
            format: 'L',
            allowInputToggle: true
        });

        new dart.DateTimePickerLink(dataAberturaInicial, dataAberturaFinal, { ignoreTime: true });
    };

    var createLinkedPickes2 = function () {
        var dataEncerramentoInicial = $DataEncerramentoInicial.datetimepicker({
            locale: moment.locale(),
            format: 'L',
            allowInputToggle: true
        });

        var dataEncerramentoFinal = $DataEncerramentoFinal.datetimepicker({
            locale: moment.locale(),
            useCurrent: false,
            format: 'L',
            allowInputToggle: true
        });

        new dart.DateTimePickerLink(dataEncerramentoInicial, dataEncerramentoFinal, { ignoreTime: true });
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
            { data: 'Lote' },
            { data: 'Nota' },
            { data: 'Fornecedor' },
            { data: 'DataAbertura', width: 100 },
            { data: 'DataEncerramento', width: 100 },
            { data: 'Atraso' },
            { data: 'Status' },
            actionsColumn
        ]
    });

    $('#dataTable').DataTable.ext.errMode = function (settings, helpPage, message) {
        let jqXHR = settings.jqXHR;
        if (jqXHR) {
            let responseJSON = jqXHR.responseJSON;
            if (responseJSON) {
                if (responseJSON.showAsToast)
                    PNotify.error({ text: responseJSON.error });
                return;
            }
        }
        alert(message);
    };

    dart.dataTables.loadFormFilterEvents();

    $("#pesquisarFornecedor").click(function () {
        $("#modalFornecedor").load(HOST_URL + "BOFornecedor/SearchModal", function () {
            $("#modalFornecedor").modal();
        });
    });

    function limparFornecedor() {
        let nomeFantasia = $("#Filter_NomeFantasiaFornecedor");
        let fornecedor = $("#Filter_IdFornecedor");
        nomeFantasia.val("");
        fornecedor.val("");
    }

    $("#limparFornecedor").click(function () {
        limparFornecedor();
    });

    $(document.body).on('click', "[action='alterarStatus']", alterarStatus);
    $(document.body).on('click', "[action='termoResponsabilidade']", termoResponsabilidade);
    $(document.body).on('click', "[action='historicoQuarentena']", historicoQuarentena);



    function alterarStatus() {
        let id = $(this).data("id");
        let $modal = $("#modalAlterarStatus");

        $.ajax({
            url: HOST_URL + CONTROLLER_PATH + "ValidarPermissao",
            data: { acao: "AtualizarStatus" },
            cache: false,
            method: "POST",
            success: function (result) {
                if (result.Success) {
                    $.ajax({
                        url: HOST_URL + CONTROLLER_PATH + "ValidarModalDetalhesQuarentena/" + id,
                        cache: false,
                        method: "POST",
                        success: function (result) {
                            if (!!result.Success) {
                                $modal.load(HOST_URL + CONTROLLER_PATH + "DetalhesQuarentena/" + id, function () {
                                    $modal.modal();

                                });
                            } else {
                                PNotify.error({ text: result.Message });
                            }
                        }
                    });
                }
                else {
                    PNotify.warning({ text: result.Message });
                }
            },
            error: function (request, status, error) {
                PNotify.error({ text: request.responseText });
            }
        });
    }

    function termoResponsabilidade() {
        var id = $(this).data("id");

        $.ajax({
            url: HOST_URL + CONTROLLER_PATH + "ValidarPermissao",
            data: { acao: "EmitirTermoResponsabilidade" },
            cache: false,
            method: "POST",
            success: function (result) {
                if (result.Success) {
                    $("#modalImpressoras").load("BOPrinter/Selecionar?idImpressaoItem=1&acao=" + id, function () {
                        $("#modalImpressoras").modal();
                    });
                } else {
                    PNotify.warning({ text: result.Message });
                }
            },
            error: function (request, status, error) {
                PNotify.error({ text: request.responseText });
            }
        });
    }

    function historicoQuarentena() {
        var id = $(this).data("id");

        $.ajax({
            url: HOST_URL + CONTROLLER_PATH + "ValidarPermissao",
            data: { acao: "ConsultarHistorico"},
            cache: false,
            method: "POST",
            success: function (result) {
                if (result.Success) {
                    $("#modalHistoricoQuarentena").load(CONTROLLER_PATH + "Historico/" + id, function () {
                        $("#modalHistoricoQuarentena").modal();
                    });
                } else {
                    PNotify.warning({ text: result.Message });
                }
            },
            error: function (request, status, error) {
                PNotify.error({ text: request.responseText });
            }
        });
    }
})();

function setFornecedor(idFornecedor, nomeFantasia) {
    $("#Filter_NomeFantasiaFornecedor").val(nomeFantasia);
    $("#Filter_IdFornecedor").val(idFornecedor);
    $("#modalFornecedor").modal("hide");
    $("#modalFornecedor").empty();
}

//Recebendo o id da quarentena no parâmetro 'acao'.
function imprimir(acao, id) {
    var idImpressora = $("#IdImpressora").val();

    $.ajax({
        url: CONTROLLER_PATH + "TermoResponsabilidade",
        method: "POST",
        cache: false,
        data: {
            idQuarentena: acao,
            IdImpressora: idImpressora
        },
        success: function (result) {
            if (result.Success) {
                PNotify.success({ text: result.Message });
            } else {
                PNotify.error({ text: result.Message });
            }

            $('#modalImpressoras').modal('toggle');
        },
        error: function (data) {
            PNotify.error({ text: "Ocorreu um erro na impressão." });
            NProgress.done();
        }
    });
}