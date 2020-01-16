(function () {
    $(".onlyNumber").mask("0#");

    var actionsColumn = dart.dataTables.renderActionsColumn(function (data, type, full, meta) {
        return [
            {
                action: 'edit',
                href: view.editUrl + '?id=' + full.UserName,
                visible: view.editVisible
            },
            {
                text: view.resetPasswordAction,
                icon: 'fa fa-key',
                attrs: { 'data-reset-url': view.resetPassUrl + '?id=' + full.UserName },
                visible: view.resetPassVisible
            },
            {
                action: 'delete',
                attrs: { 'data-delete-url': view.deleteUrl + '?id=' + full.UserName },
                visible: view.deleteVisible
            }
        ];
    });

    var options = {
        stateSave: false,
        info: false,
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
        order: [[0, "ASC"]],
        columns: [
            { data: 'Nome' },
            { data: 'UserName' },
            { data: 'Email' },
            { data: 'Status' },
            actionsColumn
        ]
    };

    $('#dataTable').dataTable(options);
    dart.dataTables.loadFormFilterEvents();

    $('#dataTable').on('click', '[data-delete-url]', function () {
        var deleteUrl = $(this).attr('data-delete-url');
        var $table = $(this).closest('table');

        dart.modalAjaxDelete.open({
            title: view.deleteModalTitle,
            message: view.deleteModalMessage,
            deleteUrl: deleteUrl,
            onConfirm: function () {
                $table.DataTable().ajax.reload(null, false);
            }
        });
    });

    $('#dataTable').on('click', '[data-reset-url]', function () {
        var resetUrl = $(this).attr('data-reset-url');
        var $table = $(this).closest('table');

        dart.modalAjaxConfirm.open({
            title: view.resetPasswordModalTitle,
            message: view.resetPasswordModalMessage,
            confirmText: view.resetPasswordAction,
            url: resetUrl,
            onConfirm: function () {
                $table.DataTable().ajax.reload(null, false);
            }
        });
    });
})();