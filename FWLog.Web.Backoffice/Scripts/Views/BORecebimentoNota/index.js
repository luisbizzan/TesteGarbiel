(function () {
    $('.onlyNumber').mask('0#');

    $("#imprimirEtiquetaConferencia").click(function () {
        $("#modalEtiquetaConferencia").load("BORecebimentoNota/DetalhesEtiquetaConferencia", function () {
            $("#modalEtiquetaConferencia").modal();
        });
    });

    $("#detalhesEntradaConferencia").click(function () {
        $("#modalDetalhesEntradaConferencia").load("BORecebimentoNota/DetalhesEntradaConferencia/26", function () {
            $("#modalDetalhesEntradaConferencia").modal();
        });
    });

    $("#imprimirRelatorio").click(function () {
        $("#modalImpressoras").load("BOPrinter/Selecionar", function () {
            $("#modalImpressoras").modal();
        });
    });

    $("#downloadRelatorio").click(function () {
        $.ajax({
            url: "/BORecebimentoNota/DownloadRelatorioNotas",
            method: "POST",
            xhrFields: {
                responseType: 'blob'
            },
            data: {
                Lote: $("#Filter_Lote").val(),
                Nota: $("#Filter_Nota").val(),
                DANFE: $("#Filter_DANFE").val(),
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
                attrs: { 'data-id': full.IdNotaFiscal, 'action': 'click' },
                icon: 'fa fa-eye',
                visible: view.registrarRecebimento
            },
            {
                text: "Registrar Recebimento",
                attrs: { 'data-id': full.IdNotaFiscal, 'action': 'click' },
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
                attrs: { 'data-id': full.IdNotaFiscal, 'action': 'click' },
                icon: 'fa fa-warning',
                visible: view.registrarRecebimento
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
            if (row.Atraso == 0)
                return row.Lote != null ? '<i class="fa fa-circle icone-status-verde"></i>' + row.Lote : '<i class="fa fa-circle icone-status-verde"></i>';

            if (row.Atraso > 0 && row.Atraso <= 2)
                return row.Lote != null ? '<i class="fa fa-circle icone-status-amarelo"></i>' + row.Lote : '<i class="fa fa-circle icone-status-amarelo"></i>';

            if (row.Atraso > 2)
                return row.Lote != null ? '<i class="fa fa-circle icone-status-vermelho"></i>' + row.Lote : '<i class="fa fa-circle icone-status-vermelho"></i>';
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
                if (data.statusText != "") {
                    PNotify.error({ text: data.statusText });
                    NProgress.done();
                }
            }
        },
        initComplete: function (settings, json) {
            dart.dataTables.addEventsForDropdownAutoposition($('#dataTable'));
            CarregarBotoesRegistrar();

            $(".row").click(function () {
                CarregarBotoesRegistrar();
            });

            $("#dataTable").on("draw.dt", function () {
                CarregarBotoesRegistrar();

                $(".row").click(function () {
                    CarregarBotoesRegistrar();
                });
            });
        },
        //stateSaveParams: function (settings, data) {
        //    dart.dataTables.saveFilterToData(data);
        //},
        //stateLoadParams: function (settings, data) {
        //    dart.dataTables.loadFilterFromData(data);
        //},
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
        let razao = $("#Filter_RazaoSocialFornecedor");
        let fornecedor = $("#Filter_IdFornecedor");
        razao.val("");
        fornecedor.val("");
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
        let userName = $("#Filter_UserNameRecebimento");
        let usuarioId = $("#Filter_IdUsuarioRecebimento");
        userName.val("");
        usuarioId.val("");
    }

    $("#limparUsuarioRecebimento").click(function () {
        limparUsuarioRecebimento();
    });

    adicionaEventos();
})();

function adicionaEventos() {
    $(document.body).on('click', "[action='conferirNota']", conferirNota);
}

function Imprimir() {
    $.ajax({
        url: "/BORecebimentoNota/ImprimirRelatorioNotas",
        method: "POST",
        data: {
            IdImpressora: $("#IdImpressora").val(),
            Lote: $("#Filter_Lote").val(),
            Nota: $("#Filter_Nota").val(),
            DANFE: $("#Filter_DANFE").val(),
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
}

function CarregarBotoesRegistrar() {
    $("[action='click']").unbind();
    $("[action='click']").click(function () {
        var id = $(this).data("id");
        $.ajax({
            url: HOST_URL + "BORecebimentoNota/ValidarModalRegistroRecebimento/" + id,
            method: "POST",            
            success: function (result) {
                if (result.Success) {
                    $("#modalRegistroRecebimento").load(HOST_URL + "BORecebimentoNota/ExibirModalRegistroRecebimento/" + id, function () {
                        $("#modalRegistroRecebimento").modal();

                        $("#ChaveAcesso").focus();

                        $('#ChaveAcesso').keypress(function (event) {
                            BuscarNotaFiscal();
                        });

                        RegistrarNotaFiscal();

                        $('.integer').mask("#0", { reverse: true });
                        $('.money').mask("#.##0,00", { reverse: true });
                    });
                } else {
                    PNotify.error({ text: result.Message });
                }
            }
        });
    });
}

function BuscarNotaFiscal() {
    var keycode = (event.keyCode ? event.keyCode : event.which);
    if (keycode === 13) {
        $(".validacaoChaveAcesso").text("");
        $(".validacaoConfirmar").text("");

        var chave = $('#ChaveAcesso').val();

        if (chave === "" || chave === undefined || chave === null) {
            return;
        }

        $.ajax({
            url: HOST_URL + "BORecebimentoNota/ValidarNotaFiscalRegistro",
            method: "POST",
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
                    });
                } else {
                    $(".validacaoChaveAcesso").text(result.Message);
                }
            }
        });
    }
}

function RegistrarNotaFiscal() {
    $("#RegistrarRecebimentoNota").click(function () {
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
    });
}

function setFornecedor(idFornecedor, razaoSocial) {
    let razao = $("#Filter_RazaoSocialFornecedor");
    let fornecedor = $("#Filter_IdFornecedor");
    razao.val(razaoSocial);
    fornecedor.val(idFornecedor);
    $("#modalFornecedor").modal("hide");
    $("#modalFornecedor").empty();
}

function setUsuarioRecebimento(idUsuario, nomeUsuario) {
    let userName = $("#Filter_UserNameRecebimento");
    let usuarioId = $("#Filter_IdUsuarioRecebimento");
    userName.val(nomeUsuario);
    usuarioId.val(idUsuario);
    $("#modalUsuarioRecebimento").modal("hide");
    $("#modalUsuarioRecebimento").empty();
}

function conferirNota() {
    debugger
}
