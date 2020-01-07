(function () {
    var actionsColumn = dart.dataTables.renderActionsColumn(function (data, type, full, meta) {
        return [
            {
                action: 'details',
                href: view.detailsUrl + '?id=' + full.IdPerfilImpressora,
                visible: view.detailsVisible
            },
            {
                action: 'edit',
                href: view.editUrl + '?id=' + full.IdPerfilImpressora,
                visible: view.editVisible
            },
            {
                action: 'delete',
                attrs: { 'data-delete-url': view.deleteUrl + '?id=' + full.IdPerfilImpressora },
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
        columns: [
            { data: 'IdPerfilImpressora' },
            { data: 'Nome' },
            { data: 'Status' },
            actionsColumn
        ]
    });

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