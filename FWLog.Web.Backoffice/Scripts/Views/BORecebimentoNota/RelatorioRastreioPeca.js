(function () {
    dart.dataTables.loadFormFilterEvents();

    var $CompraInicial = $('#Filter_DataCompraMinima').closest('.date');
    var $CompraFinal = $('#Filter_DataCompraMaxima').closest('.date');
    var $RecebidoInicial = $('#Filter_DataRecebimentoMinima').closest('.date');
    var $RecebidoFinal = $('#Filter_DataRecebimentoMaxima').closest('.date');

    var createLinkedPickersCompra = function () {
        var dataInicial = $CompraInicial.datetimepicker({
            locale: moment.locale(),
            format: 'L',
            allowInputToggle: true
        });

        var dataFinal = $CompraFinal.datetimepicker({
            locale: moment.locale(),
            useCurrent: false,
            format: 'L',
            allowInputToggle: true
        });

        new dart.DateTimePickerLink(dataInicial, dataFinal, { ignoreTime: true });
    };

    var createLinkedPickesRecebido = function () {
        var dataInicial = $RecebidoInicial.datetimepicker({
            locale: moment.locale(),
            format: 'L',
            allowInputToggle: true
        });

        var dataFinal = $RecebidoFinal.datetimepicker({
            locale: moment.locale(),
            useCurrent: false,
            format: 'L',
            allowInputToggle: true,
        });

        new dart.DateTimePickerLink(dataInicial, dataFinal, { ignoreTime: true });
    }

    createLinkedPickersCompra();
    createLinkedPickesRecebido();

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
            { data: 'IdLote' },
            { data: 'NroNota' },
            { data: 'ReferenciaPronduto' },
            { data: 'DataRecebimento' },
            { data: 'QtdCompra' },
            { data: 'QtdRecebida' }
        ]
    });

    $('#dataTable').dataTable.error = function (settings, helpPage, message) {
        console.log(message)
    };
})();