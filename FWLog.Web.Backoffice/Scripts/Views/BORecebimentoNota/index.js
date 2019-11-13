(function () {
    $("#imprimirEtiquetaConferencia").click(function () {
        $("#modalEtiquetaConferencia").load("BORecebimentoNota/DetalhesEtiquetaConferencia", function () {
            $("#modalEtiquetaConferencia").modal();
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
                action: 'details',
                href: view.detailsUrl + '?id=' + full.Id,
                visible: view.detailsVisible
            },
            {
                action: 'edit',
                href: view.editUrl + '?id=' + full.Id,
                visible: view.editVisible
            },
            {
                text: "Registrar Recebimento",
                attrs: { 'data-id': full.IdNotaFiscal, 'action': 'click' },
                icon: 'fa fa-exclamation-circle',
                visible: view.registrarRecebimento
            },
            {
                action: 'delete',
                attrs: { 'data-delete-url': view.deleteUrl + '?id=' + full.Id },
                visible: view.deleteVisible
            },
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

        var prazoInicial = $PrazoInicial.datetimepicker({
            locale: moment.locale(),
            useCurrent: false,
            format: 'L',
            allowInputToggle: true
        });

        var prazoFinal = $PrazoFinal.datetimepicker({
            locale: moment.locale(),
            useCurrent: false,
            format: 'L',
            allowInputToggle: true
        });

        new dart.DateTimePickerLink(dataInicial, dataFinal, prazoInicial, prazoFinal, { ignoreTime: true });
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
        stateSaveParams: function (settings, data) {
            dart.dataTables.saveFilterToData(data);
        },
        stateLoadParams: function (settings, data) {
            dart.dataTables.loadFilterFromData(data);
        },
        columns: [
            { data: 'Lote' },
            { data: 'Nota' },
            { data: 'QuantidadePeca' },
            { data: 'QuantidadeVolume' },
            { data: 'Atraso' },
            { data: 'Prazo' },
            { data: 'Fornecedor' },
            { data: 'Status' },
            actionsColumn
        ]
    });
    dart.dataTables.loadFormFilterEvents();
})();

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

        if ($($(this).parents("tr").children()[0]).text() !== "") {
            alert("O recebimento da nota fiscal já foi registrado.");
            return;
        }


        $("#modalRegistroRecebimento").load(HOST_URL + "BORecebimentoNota/ExibirModalRegistroRecebimento/" + $(this).data("id"), function () {
            $("#modalRegistroRecebimento").modal();

            $("#ChaveAcesso").focus();

            $('#ChaveAcesso').keypress(function (event) {
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
            });

            $("#RegistrarRecebimentoNota").click(function () {        
                $(".validacaoConfirmar").text("");
                if (!($("#QtdVolumes").val() > 0) || (!$("#NotaFiscalPesquisada").val() === true))
                {
                    $(".validacaoConfirmar").text("Selecione a nota fiscal e insira a quantidade de volumes para confirmar o recebimento.");
                    return;
                }

                $.ajax({
                    url: HOST_URL + "BORecebimentoNota/RegistrarRecebimentoNota/",
                    method: "POST",
                    data: {                    
                        idNotaFiscal: $("#IdNotaFiscal").val(),
                        dataRecebimento: $("#DataAtual").val(),                    
                        qtdVolumes: $("#QtdVolumes").val()
                    },
                    success: function (result) {
                        if (result.Success) {
                            $(".close").click();
                            $("#dataTable").DataTable().ajax.reload(); 
                            PNotify.success({ text: result.Message });
                        } else {
                            PNotify.error({ text: result.Message });
                        }
                    }
                });
            });

        });
    });
}
