(function () {    
    var actionsColumn = dart.dataTables.renderActionsColumn(function (data, type, full, meta) {
        return [
            {
                action: 'details',
                href: view.detailsUrl + '?id=' + full.IdNivelArmazenagem,
                visible: view.detailsVisible
            },
            {
                action: 'edit',
                href: view.editUrl + '?id=' + full.IdNivelArmazenagem,
                visible: view.editVisible
            },
            {
                action: 'delete',
                attrs: { 'data-delete-url': view.deleteUrl + '?id=' + full.IdNivelArmazenagem },
                visible: view.deleteVisible
            },
        ];
    });

    var options = {
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
        order: [[0, "desc"]],
        columns: [
            { data: 'IdPontoArmazenagem' },
            { data: 'NivelArmazenagem' },
            { data: 'Descricao' },
            actionsColumn
        ]
    };

    $('#dataTable').DataTable(options);

    var load = function () {
        dart.dataTables.loadFormFilterEvents();
        createLinkedPickers();
    };

    load();

})();