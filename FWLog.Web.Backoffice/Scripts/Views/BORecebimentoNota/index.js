(function () {
    $.validator.setDefaults({ ignore: null });
    $('.onlyNumber').mask('0#');
    $("dateFormat").mask("99/99/9999");
    $('.hourMinute').mask("99:99", { reverse: true });

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

    $("#Filter_TempoInicial").blur(function () {
        if ($(this).val() === ":") {
            $(this).val("");
        }
    });

    $("#Filter_InicialInicial").blur(function () {
        if ($(this).val() === ":") {
            $(this).val("");
        }
    });

    $("#imprimirEtiquetaConferencia").click(function () {
        $("#modalEtiquetaConferencia").load("BORecebimentoNota/DetalhesEtiquetaConferencia", function () {
            $("#modalEtiquetaConferencia").modal();
        });
    });

    $("#imprimirRelatorio").click(function () {
        $("#modalImpressoras").load("BOPrinter/Selecionar?idImpressaoItem=1&acao=notas", function () {
            $("#modalImpressoras").modal();
        });
    });

    $("#downloadRelatorio").click(function () {
        $.ajax({
            url: "/BORecebimentoNota/DownloadRelatorioNotas",
            method: "POST",
            cache: false,
            xhrFields: {
                responseType: 'blob'
            },
            data: {
                Lote: $("#Filter_Lote").val(),
                Nota: $("#Filter_Nota").val(),
                ChaveAcesso: $("#Filter_ChaveAcesso").val(),
                IdStatus: $("#Filter_IdStatus").val(),
                DataInicial: $("#Filter_DataInicial").val(),
                DataFinal: $("#Filter_DataFinal").val(),
                PrazoInicial: $("#Filter_PrazoInicial").val(),
                PrazoFinal: $("#Filter_PrazoFinal").val(),
                IdFornecedor: $("#Filter_IdFornecedor").val(),
                Atraso: $("#Filter_Atraso").val(),
                QuantidadePeca: $("#Filter_QuantidadePeca").val(),
                QuantidadeVolume: $("#Filter_QuantidadeVolume").val(),
                IdUsuarioRecebimento: $("#Filter_IdUsuarioRecebimento").val(),
                IdUsuarioConferencia: $("#Filter_IdUsuarioConferencia").val(),
                TempoInicial: $("#Filter_TempoInicial").val(),
                TempoFinal: $("#Filter_TempoFinal").val()
            },
            success: function (data) {
                var a = document.createElement('a');
                var url = window.URL.createObjectURL(data);

                a.href = url;
                a.download = 'Relatório Recebimento Notas.pdf';
                document.body.append(a);
                a.click();
                a.remove();
                window.URL.revokeObjectURL(url);
            }
        });
    });

    var $DataInicial = $('#Filter_DataInicial').closest('.date');
    var $DataFinal = $('#Filter_DataFinal').closest('.date');
    var $PrazoInicial = $('#Filter_PrazoInicial').closest('.date');
    var $PrazoFinal = $('#Filter_PrazoFinal').closest('.date');

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

    var createLinkedPickes2 = function () {
        var prazoInicial = $PrazoInicial.datetimepicker({
            locale: moment.locale(),
            format: 'L',
            allowInputToggle: true
        });

        var prazoFinal = $PrazoFinal.datetimepicker({
            locale: moment.locale(),
            useCurrent: false,
            format: 'L',
            allowInputToggle: true
        });

        new dart.DateTimePickerLink(prazoInicial, prazoFinal, { ignoreTime: true });
    };

    createLinkedPickers();
    createLinkedPickes2();

    var iconeStatus = function (data, type, row) {
        if (type === 'display') {
            var nomeCor,
                lote = row.Lote || '';

            if (row.Atraso === 0)
                nomeCor = 'verde';

            if (row.Atraso > 0) {
                if (row.Atraso > 2) {
                    nomeCor = 'vermelho';
                } else {
                    nomeCor = 'amarelo';
                }
            }

            return `<i class="fa fa-circle icone-status-${nomeCor}"></i>${lote}`;
        }

        return data;
    };

    var actionsColumn = dart.dataTables.renderActionsColumn(function (data, type, full, meta) {
        var visivelDivergencia = view.tratarDivergencias && full.IdLoteStatus === 5;
        var visivelProcessamento = view.tratarDivergencias && (full.IdLoteStatus === 9 || full.IdLoteStatus === 10 || full.IdLoteStatus === 11);
        var visivelRegistrarRecibimento = view.registrarRecebimento && full.IdLoteStatus === 1;
        var visivelConferirLote = view.conferirLote && (full.IdLoteStatus === 2 || full.IdLoteStatus === 3);

        return [
            {
                text: "Detalhes da Nota",
                attrs: { 'data-id': full.IdNotaFiscal, 'action': 'detalhesNota' },
                icon: 'fa fa-eye',
                visible: view.detailsVisible
            },
            {
                text: "Registrar Recebimento",
                attrs: { 'data-id': full.IdNotaFiscal, 'action': 'registrarRecebimento' },
                icon: 'fa fa-pencil-square',
                visible: visivelRegistrarRecibimento
            },
            {
                text: "Conferir Nota",
                attrs: { 'data-id': full.IdNotaFiscal, 'action': 'conferirNota' },
                icon: 'fa fa-check-square-o',
                visible: visivelConferirLote
            },
            {
                text: "Tratar Divergências",
                attrs: { 'data-id': full.IdNotaFiscal, 'action': 'tratarDivergencias' },
                icon: 'fa fa-warning',
                visible: visivelDivergencia
            },
            {
                text: "Sincronizar Integração Sankhya",
                attrs: { 'data-id': full.IdNotaFiscal, 'action': 'exibirProcessamento' },
                icon: 'fa fa-gears',
                visible: visivelProcessamento
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
                if (!data.statusText) {
                    PNotify.error({ text: data.statusText });
                    NProgress.done();
                }
            }
        },
        initComplete: function (settings, json) {
            dart.dataTables.addEventsForDropdownAutoposition($('#dataTable'));
        },
        columns: [
            { data: 'Lote', render: iconeStatus },
            { data: 'Nota' },
            { data: 'QuantidadePeca' },
            { data: 'QuantidadeVolume' },
            { data: 'RecebidoEm' },
            { data: 'Atraso' },
            { data: 'Prazo' },
            { data: 'Fornecedor' },
            { data: 'Status' },
            actionsColumn
        ]
    });

    $('#dataTable').dataTable.error = function (settings, helpPage, message) {
        console.log(message);
    };

    $("#dataTable").on('click', "[action='conferirNota']", conferirNota);
    $("#dataTable").on('click', "[action='registrarRecebimento']", registrarRecebimento);
    $("#dataTable").on('click', "[action='detalhesNota']", detalhesNota);
    $("#dataTable").on('click', "[action='tratarDivergencias']", tratarDivergencias);
    $("#dataTable").on('click', "[action='exibirDivergencias']", exibirDivergencias);
    $("#dataTable").on('click', "[action='exibirProcessamento']", exibirProcessamento);

    dart.dataTables.loadFormFilterEvents();

    $("#pesquisarFornecedor").click(function () {
        $("#modalFornecedor").load(HOST_URL + "BOFornecedor/SearchModal", function () {
            $("#modalFornecedor").modal();
        });
    });

    function limparFornecedor() {
        $("#Filter_RazaoSocialFornecedor").val("");
        $("#Filter_IdFornecedor").val("");
    }

    $("#limparFornecedor").click(function () {
        limparFornecedor();
    });

    $("#pesquisarUsuarioRecebimento").click(function () {
        $("#modalUsuarioRecebimento").load(HOST_URL + "BOAccount/SearchModal/Recebimento", function () {
            $("#modalUsuarioRecebimento").modal();
        });
    });

    function limparUsuarioRecebimento() {
        $("#Filter_UserNameRecebimento").val("");
        $("#Filter_IdUsuarioRecebimento").val("");
    }

    $("#limparUsuarioRecebimento").click(function () {
        limparUsuarioRecebimento();
    });

    $("#pesquisarUsuarioConferencia").click(function () {
        $("#modalUsuarioConferencia").load(HOST_URL + "BOAccount/SearchModal/Conferencia", function () {
            $("#modalUsuarioConferencia").modal();
        });
    });

    function limparUsuarioConferencia() {
        $("#Filter_UserNameConferencia").val("");
        $("#Filter_IdUsuarioConferencia").val("");
    }

    $("#limparUsuarioConferencia").click(function () {
        limparUsuarioConferencia();
    });
})();

function imprimir(acao, id) {
    switch (acao) {
        case 'notas':
            $.ajax({
                url: "/BORecebimentoNota/ImprimirRelatorioNotas",
                method: "POST",
                data: {
                    IdImpressora: $("#IdImpressora").val(),
                    Lote: $("#Filter_Lote").val(),
                    Nota: $("#Filter_Nota").val(),
                    ChaveAcesso: $("#Filter_ChaveAcesso").val(),
                    IdStatus: $("#Filter_ListaStatus").val(),
                    DataInicial: $("#Filter_DataInicial").val(),
                    DataFinal: $("#Filter_DataFinal").val(),
                    PrazoInicial: $("#Filter_PrazoInicial").val(),
                    PrazoFinal: $("#Filter_PrazoFinal").val(),
                    IdFornecedor: $("#Filter_IdFornecedor").val(),
                    Atraso: $("#Filter_Atraso").val(),
                    QuantidadePeca: $("#Filter_QuantidadePeca").val(),
                    Volume: $("#Filter_Volume").val(),
                    IdUsuarioRecebimento: $("#Filter_IdUsuarioRecebimento").val(),
                    IdUsuarioConferencia: $("#Filter_IdUsuarioConferencia").val(),
                    TempoInicial: $("#Filter_TempoInicial").val(),
                    TempoFinal: $("#Filter_TempoFinal").val()
                },
                success: function (result) {
                    mensagemImpressao(result);
                    $('#modalImpressoras').modal('toggle');
                    waitingDialog.hide();
                }
            });
            break;
        case 'entconf':
            $.ajax({
                url: "/BORecebimentoNota/ImprimirDetalhesEntradaConferencia",
                method: "POST",
                data: {
                    IdImpressora: $("#IdImpressora").val(),
                    IdNotaFiscal: id
                },
                success: function (result) {
                    mensagemImpressao(result);
                    $('#modalImpressoras').modal('toggle');
                    waitingDialog.hide();
                }
            });
            break;
        case 'etqrecebimento':
            $.ajax({
                url: "/BORecebimentoNota/ImprimirEtiquetaRecebimento",
                method: "POST",
                data: {
                    IdImpressora: $("#IdImpressora").val(),
                    IdNotaFiscal: id
                },
                success: function (result) {
                    mensagemImpressao(result);
                    $('#modalImpressoras').modal('toggle');
                    waitingDialog.hide();
                }
            });
            break;
    }
}

function mensagemImpressao(result) {
    if (result.Success) {
        PNotify.success({ text: result.Message });
    } else {
        PNotify.error({ text: result.Message });
    }
}

function detalhesNota() {
    var id = $(this).data("id");
    let $modalDetalhesEntradaConferencia = $("#modalDetalhesEntradaConferencia");

    $modalDetalhesEntradaConferencia.load(CONTROLLER_PATH + "DetalhesEntradaConferencia/" + id, function () {
        $modalDetalhesEntradaConferencia.modal();
    });
}

function registrarRecebimento() {
    let id = $(this).data("id");

    $.ajax({
        url: HOST_URL + CONTROLLER_PATH + "ValidarModalRegistroRecebimento/" + id,
        method: "POST",
        cache: false,
        success: function (result) {
            let $modalRegistroRecebimento = $("#modalRegistroRecebimento");
            if (result.Success) {
                $modalRegistroRecebimento.load(HOST_URL + CONTROLLER_PATH + "ExibirModalRegistroRecebimento/" + id, function () {
                    $modalRegistroRecebimento.modal();
                    $("#ChaveAcesso").focus();
                    $('#ChaveAcesso').keypress(function (event) {
                        BuscarNotaFiscal();
                    });
                    $("#RegistrarRecebimentoNota").click(function () {
                        RegistrarNotaFiscal();
                    });
                    $('.integer').mask("#0", { reverse: true });
                    $('.money').mask("#.##0,00", { reverse: true });
                });
            } else {
                PNotify.info({ text: result.Message });
            }
        }
    });
}

function BuscarNotaFiscal() {
    var keycode = event.keyCode || event.which;

    if (keycode === 13) {
        $(".validacaoChaveAcesso").text("");
        $(".validacaoConfirmar").text("");

        var chave = $('#ChaveAcesso').val();

        if (!chave) {
            return;
        }

        $.ajax({
            url: HOST_URL + "BORecebimentoNota/ValidarNotaFiscalRegistro",
            method: "POST",
            cache: false,
            data: {
                idNotaFiscal: $("#IdNotaFiscal").val(),
                chaveAcesso: chave
            },
            success: function (result) {
                if (result.Success) {
                    $("#RegistroRecebimentoDetalhes").load("BORecebimentoNota/CarregarDadosNotaFiscalRegistro/" + $("#IdNotaFiscal").val(), function () {
                        $('.integer').mask("#0", { reverse: true });
                        $('.money').mask("#.##0,00", { reverse: true });
                        $('#ChaveAcesso').attr("disabled", true);

                        $('#QtdVolumes').keypress(function (event) {
                            var keycode = event.keyCode || event.which;

                            if (keycode === 13) {
                                RegistrarNotaFiscal();
                            }
                        });

                        waitingDialog.hide();
                        $("#QtdVolumes").focus();
                    });
                } else {
                    $('#ChaveAcesso').val("");
                    $(".validacaoChaveAcesso").text(result.Message);
                }
            }
        });
    }
}

function RegistrarNotaFiscal() {
    $(".validacaoConfirmar").text("");

    if ($("#QtdVolumes").val() === "" || $("#NotaFiscalPesquisada").val() === "False" || $("#IdNotaFiscal").val() <= 0) {
        $(".validacaoConfirmar").text("Selecione a nota fiscal e insira a quantidade de volumes para confirmar o recebimento.");
        return;
    }

    $.ajax({
        url: HOST_URL + "BORecebimentoNota/RegistrarRecebimentoNota/",
        method: "POST",
        data: {
            idNotaFiscal: $("#IdNotaFiscal").val(),
            dataRecebimento: $("#DataAtual").val(),
            qtdVolumes: $("#QtdVolumes").val(),
            notaFiscalPesquisada: $("#NotaFiscalPesquisada").val() === "True" ? true : false
        },
        success: function (result) {
            if (result.Success) {
                PNotify.success({ text: result.Message });
                $(".close").click();
                $("#modalImpressoras").load("BOPrinter/Selecionar?tipo=zebra&acao=etqrecebimento&id=" + $("#IdNotaFiscal").val(), function () {
                    $("#modalImpressoras").modal();
                });
                $("#dataTable").DataTable().ajax.reload();
            } else {
                $(".validacaoConfirmar").text(result.Message);
            }
        }
    });
}

function setFornecedor(idFornecedor, razaoSocial) {
    $("#Filter_RazaoSocialFornecedor").val(razaoSocial);
    $("#Filter_IdFornecedor").val(idFornecedor);
    $("#modalFornecedor").modal("hide");
    $("#modalFornecedor").empty();
}

function setUsuario(idUsuario, nomeUsuario, origem) {
    if (origem === "Recebimento") {
        $("#Filter_UserNameRecebimento").val(nomeUsuario);
        $("#Filter_IdUsuarioRecebimento").val(idUsuario);
        $("#modalUsuarioRecebimento").modal("hide");
        $("#modalUsuarioRecebimento").empty();
    } else {
        $("#Filter_UserNameConferencia").val(nomeUsuario);
        $("#Filter_IdUsuarioConferencia").val(idUsuario);
        $("#modalUsuarioConferencia").modal("hide");
        $("#modalUsuarioConferencia").empty();
    }
}

function conferirNota() {
    let id = $(this).data("id");
    let $modal = $("#modalConferencia");

    $.ajax({
        url: HOST_URL + CONTROLLER_PATH + "ValidarInicioConferencia/" + id,
        cache: false,
        method: "POST",
        success: function (result) {
            if (result.Success) {
                if (!conferirAutomatico()) {
                    $modal.load(HOST_URL + CONTROLLER_PATH + "EntradaConferencia/" + id, function (result) {
                        $modal.modal();
                        $("#Referencia").focus();
                    });
                }
            } else {
                PNotify.info({ text: result.Message });
            }
        },
        error: function (request, status, error) {
            PNotify.info({ text: request.responseText });
        }
    });
}


function conferirAutomatico() {
    var rtn = false;

    $.ajax({
        url: HOST_URL + CONTROLLER_PATH + "ValidarConferenciaAutomatica/" + id,
        cache: false,
        method: "POST",
        success: function (result) {
            if (result.Success) {
                if (result.Data) {
                    rtn = result.Data;

                    PNotify.success({ text: result.Message });
                }
            } else {
                PNotify.info({ text: result.Message });
            }
        },
        error: function (request, status, error) {
            PNotify.info({ text: request.responseText });
        }
    });

    return rtn;
}

function tratarDivergencias() {
    let id = $(this).data("id");
    let $modal = $("#modalDivergencia");

    $modal.load(HOST_URL + CONTROLLER_PATH + "TratarDivergencia/" + id, function () {
        $modal.modal();
    });
}

function exibirDivergencias() {
    let id = $(this).data("id");
    let $modal = $("#modalDivergencia");

    $modal.load(HOST_URL + CONTROLLER_PATH + "ExibirDivergencia/" + id, function () {
        $modal.modal();
    });
}

function exibirProcessamento() {
    let id = $(this).data("id");

    let $modal = $("#modalProcessamentoTratativaDivergencia");

    $modal.load(HOST_URL + CONTROLLER_PATH + "ResumoProcessamentoDivergencia/" + id, function () {
        $modal.modal();
        $('input').iCheck({ checkboxClass: 'icheckbox_flat-green' });
    });
}


