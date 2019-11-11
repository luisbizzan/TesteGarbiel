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