(function () {
    $('#dataTable').DataTable({
        ajax: {
            "url": view.RecebimentopageDataUrl,
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
        //deferLoading: 0,
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
            { data: 'NomeUsuario' },
            { data: 'NotasRecebidasUsuario' },
            { data: 'PecasRecebidasUsuario' },
            { data: 'NotasRecebidas', width: 100 },
            { data: 'PecasRecebidas', width: 100 },
            { data: 'Percentual', width: 100 },
            { data: 'Ranking', width: 100 }
        ]
    });

    $('#dataTable').dataTable.error = function (settings, helpPage, message) {
        console.log(message)
    };
})();