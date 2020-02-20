(function () {

    var actionsColumn = dart.dataTables.renderActionsColumn(function (data, type, full, meta) {
        return [
            {
                action: 'details',
                href: view.detailsUrl + '?id=' + full.IdGarantia,
                visible: view.detailsVisible
            }
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
        columns: [
            // TODO: Todas as proriedades da classe GarantiaListItemViewModel devem estar listadas aqui 
            { data: 'IdGarantia' },
            { data: 'IdCliente' },
            { data: 'IdTransportadora' },
            actionsColumn
        ]
    });

    // Delete
    $('#dataTable').on('click', '[data-delete-url]', function () {

        var deleteUrl = $(this).attr('data-delete-url');
        var $table = $(this).closest('table');

        dart.modalAjaxDelete.open({
            deleteUrl: deleteUrl,
            onConfirm: function () {
                $table.DataTable().ajax.reload(null, false);
            }
        });
    });

    dart.dataTables.loadFormFilterEvents();

})();