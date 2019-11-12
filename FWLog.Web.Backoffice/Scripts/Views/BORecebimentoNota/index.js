(function () { 
    $("#imprimirEtiquetaConferencia").click(function () {
        $("#modalEtiquetaConferencia").load("BORecebimentoNota/DetalhesEtiquetaConferencia", function () {
            $("#modalEtiquetaConferencia").modal();
        });
    });

    $("#RegistrarRecebimentoNota").click(function () {
        $("#modalEtiquetaConferencia").load("BORecebimentoNota/RegistrarRecebimentoNota", function () {
           //Definir
        });
    });

    //Alterar chamada
    $("#Registro").click(function () {
        $("#modalRegistroRecebimento").load("BORecebimentoNota/ExibirModalRegistroRecebimento", function () {
            $("#modalRegistroRecebimento").modal();

            $('#ChaveAcesso').keypress(function (event) {
                var keycode = (event.keyCode ? event.keyCode : event.which);
                if (keycode === 13) {
                    $("#RegistroRecebimentoDetalhes").load("BORecebimentoNota/CarregarDadosNotaFiscalRegistro?chaveAcesso=" + $('#ChaveAcesso').val(), function () { });
                }
            });
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
                IdStatus: $("#Filter_IdStatus").val(),
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

    var $DataInicial = $('#Filter_DataInicial').closest('.date');
    var $DataFinal = $('#Filter_DataFinal').closest('.date');
    var $PrazoInicial = $('#Filter_PrazoInicial').closest('.date');
    var $PrazoFinal = $('#Filter_PrazoFinal').closest('.date');

    var createLinkedPickers = function () {

        var pickerStart = $DataInicial.datetimepicker({
            locale: moment.locale(),
            format: 'L',
            allowInputToggle: true
        });

        var pickerEnd = $DataFinal.datetimepicker({
            locale: moment.locale(),
            useCurrent: false,
            format: 'L',
            allowInputToggle: true
        });

        new dart.DateTimePickerLink(pickerStart, pickerEnd, { ignoreTime: true });
    };

    createLinkedPickers();

    $("#limpaPesquisaRecebimentoNota").click(function () {
        var $form = $('[data-filter="form"]');

        $form[0].reset();
        $form.find('input').val('');

        var table = $('#dataTable').DataTable();

        table.clear();
        table.destroy();

        $('#dataTable').dataTable(options);
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
        columns: [
            { data: 'Lote' },
            { data: 'Nota' },
            { data: 'QuantidadePeca' },
            { data: 'QuantidadeVolume' },
            { data: 'Atraso' },
            { data: 'Prazo' },
            { data: 'Fornecedor' },
            { data: 'Status' }
        ]
    });
    dart.dataTables.loadFormFilterEvents();
})();