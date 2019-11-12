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