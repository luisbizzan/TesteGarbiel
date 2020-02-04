(function () {

    var actionsColumn = dart.dataTables.renderActionsColumn(function (data, type, full, meta) {
        return [
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
            { data: 'Descricao' },
            { data: 'Status', width: 100  },
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