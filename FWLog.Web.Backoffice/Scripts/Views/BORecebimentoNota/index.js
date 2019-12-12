(function () {
    $.validator.setDefaults({ ignore: null });

    $('.onlyNumber').mask('0#');
    $("dateFormat").mask("99/99/9999");

    $("#imprimirEtiquetaConferencia").click(function () {
        $("#modalEtiquetaConferencia").load("BORecebimentoNota/DetalhesEtiquetaConferencia", function () {
            $("#modalEtiquetaConferencia").modal();
        });
    });

    $("#imprimirRelatorio").click(function () {
        $("#modalImpressoras").load("BOPrinter/Selecionar/2", function () {
            $("#relatorioImprimir").val("Notas");
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
                IdUsuarioConferencia: $("#Filter_IdUsuarioConferencia").val()
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

    var actionsColumn = dart.dataTables.renderActionsColumn(function (data, type, full, meta) {
        return [
            {
                text: "Detalhes da Nota",
                attrs: { 'data-id': full.IdNotaFiscal, 'action': 'detalhesNota' },
                icon: 'fa fa-eye',
                visible: view.registrarRecebimento
            },
            {
                text: "Registrar Recebimento",
                attrs: { 'data-id': full.IdNotaFiscal, 'action': 'registrarRecebimento' },
                icon: 'fa fa-pencil-square',
                visible: view.registrarRecebimento
            },
            {
                text: "Conferir Nota",
                attrs: { 'data-id': full.IdNotaFiscal, 'action': 'conferirNota' },
                icon: 'fa fa-check-square-o',
                visible: view.registrarRecebimento
            },
            {
                text: "Tratar Divergência",
                attrs: { 'data-id': full.IdNotaFiscal, 'action': 'tratarDivergencias' },
                icon: 'fa fa-warning',
                visible: view.tratarDivergencias
            }
        ];
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
    }

    createLinkedPickers();
    createLinkedPickes2();

    var iconeStatus = function (data, type, row) {
        if (type === 'display') {
            var nomeCor,
                lote = row.Lote || '';

            if (row.Atraso == 0)
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

    $('#dataTable').DataTable({
        ajax: {
            "url": view.pageDataUrl,
            "type": "POST",
            "data": function (data) {
                dart.dataTables.saveFilterToData(data);
            },
            "error": function (data) {
                if (!!(data.statusText)) {
                    PNotify.error({ text: data.statusText });
                    NProgress.done();
                }
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
        console.log(message)
    };

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
        $("#modalUsuarioRecebimento").load(HOST_URL + "BOAccount/SearchModal", function () {
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
        $("#modalUsuarioConferencia").load(HOST_URL + "BOAccount/SearchModal", function () {
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

    adicionaEventos();
})();

function adicionaEventos() {
    $(document.body).on('click', "[action='conferirNota']", conferirNota);
    $(document.body).on('click', "[action='registrarRecebimento']", registrarRecebimento);
    $(document.body).on('click', "[action='detalhesNota']", detalhesNota);
    $(document.body).on('click', "[action='tratarDivergencias']", tratarDivergencias);
}

function imprimir() {
    if ($("#relatorioImprimir").val() === 'Notas') {
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
                Volume: $("#Filter_Volume").val()
            },
            success: function () {
                $("#btnFechar").click();
            }
        });
    } else {
        $.ajax({
            url: "/BORecebimentoNota/ImprimirDetalhesEntradaConferencia",
            method: "POST",
            data: {
                IdImpressora: $("#IdImpressora").val(),
                IdNotaFiscal: $("#idNotaFiscalImprimir").val()
            },
            success: function () {
                $("#btnFechar").click();
            }
        });
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
    var keycode = (event.keyCode || event.which);

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
                            var keycode = (event.keyCode || event.which);

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
            notaFiscalPesquisada: $("#NotaFiscalPesquisada").val() == "True" ? true : false
        },
        success: function (result) {
            if (result.Success) {
                $(".close").click();
                $("#dataTable").DataTable().ajax.reload();
                PNotify.success({ text: result.Message });
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

function setUsuarioRecebimento(idUsuario, nomeUsuario) {
    $("#Filter_UserNameRecebimento").val(nomeUsuario);
    $("#Filter_IdUsuarioRecebimento").val(idUsuario);

    $("#modalUsuarioRecebimento").modal("hide");
    $("#modalUsuarioRecebimento").empty();
}

function conferirNota() {
    let id = $(this).data("id");
    let $modal = $("#modalConferencia");

    $.ajax({
        url: HOST_URL + "BORecebimentoNota/ValidarModalRegistroConferencia/" + id,
        cache: false,
        method: "POST",
        success: function (result) {
            if (!!result.Success) {
                $modal.load(HOST_URL + CONTROLLER_PATH + "ExibirModalRegistroConferencia/" + id, function () {
                    $modal.on('shown.bs.modal', function () {
                        $('#Referencia').focus();
                    });

                    $modal.modal('show');
                });
            } else {
                PNotify.info({ text: result.Message });
            }
        }
    });
}

function tratarDivergencias() {
    let id = $(this).data("id");
    let $modal = $("#modalTratarDivergencia");

    $modal.load(HOST_URL + CONTROLLER_PATH + "TratarDivergencia/" + id, function () {
        $modal.modal();
    });
}
