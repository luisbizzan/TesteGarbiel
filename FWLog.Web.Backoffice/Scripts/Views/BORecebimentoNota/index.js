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
                attrs: { 'data-id': full.IdNotaFiscal },
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

            $("#dataTable").on("draw.dt", function () {
                CarregarBotoesRegistrar();
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
    $(".fa-exclamation-circle ").click(function () {
        $("#modalRegistroRecebimento").load("BORecebimentoNota/ExibirModalRegistroRecebimento/" + $(this).parent().data("id"), function () {
            $("#modalRegistroRecebimento").modal();

            $('#ChaveAcesso').keypress(function (event) {
                var keycode = (event.keyCode ? event.keyCode : event.which);
                if (keycode === 13) {
                    var chave = $('#ChaveAcesso').val();

                    if (chave === "" || chave === undefined || chave === null) {
                        return;
                    }

                    var idNotaFiscal = $("#IdNotaFiscal").val();

                    $("#RegistroRecebimentoDetalhes").load("BORecebimentoNota/CarregarDadosNotaFiscalRegistro?chaveAcesso=" + chave + "&idNotaFiscal=" + idNotaFiscal, function () {
                    });
                }
            });
        });

        $("#RegistrarRecebimentoNota").click(function () {
            console.log($("#IdNotaFiscal").val());
            console.log($("#ValorFrete").val());
            console.log($("#NumeroConhecimento").val());
            console.log($("#TransportadoraNome").val());
            console.log($("#Peso").val());
            console.log($("#QtdVolumes").val());
            /*$.ajax({
                url: HOST_URL + "BORecebimentoNota/RegistrarRecebimentoNota/",
                method: "POST",
                data: {                    
                    idNotaFiscal: $("#IdNotaFiscal").val(),
                    valorFrete: $("#ValorFrete").val(),
                    nroConhecimento: $("#NumeroConhecimento").val(),
                    idTransportadora: $("#TransportadoraNome").val(),
                    peso: $("#Peso").val(),
                    volumes: $("#QtdVolumes").val(),
                },
                success: function () {
                    $("#btnFechar").click();
                }
            });*/
        });
    });
}