(function () {

    var actionsColumn = dart.dataTables.renderActionsColumn(function (data, type, full, meta) {
        return [
            {
                action: 'edit',
                href: view.editUrl + '?id=' + full.IdMotivoLaudo,
                visible: view.editVisible
            },
        ];
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
        stateSaveParams: function (settings, data) {
            dart.dataTables.saveFilterToData(data);
        },
        stateLoadParams: function (settings, data) {
            dart.dataTables.loadFilterFromData(data);
        },
        order: [[1, "desc"]],
        columns: [
            { data: 'IdMotivoLaudo', width: 60 },
            { data: 'Descricao' },
            { data: 'Status', width: 100 },
            actionsColumn
        ]
    });

    dart.dataTables.loadFormFilterEvents();

})();