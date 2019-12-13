(function () {

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

    //dart.dataTables.loadFormFilterEvents();

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