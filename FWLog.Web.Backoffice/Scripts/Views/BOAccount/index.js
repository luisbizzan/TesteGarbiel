(function () {

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

    var multipleActions = [
        {
            text: view.deleteAction,
            icon: 'fa fa-trash-o',
            visible: view.deleteVisible,
            onClick: function (itemsChecked) {
                if (itemsChecked.length > 0) {
                    var params = itemsChecked.join('&userNameList=');

                    dart.modalAjaxConfirm.open({
                        title: view.deleteModalTitle,
                        message: view.deleteMassModalMessage.replace('{0}', itemsChecked.length),
                        confirmText: view.deleteAction,
                        url: view.deleteMassUrl + '?userNameList=' + params,
                        onConfirm: function () {
                            $('table').DataTable().ajax.reload(null, false);
                            $('.all-check-dataTable').prop('checked', false);
                        }
                    });
                }
            }
        }
    ];

    // DataTable
    var options = dart.dataTables.multipleAction('UserName', '#dataTable', multipleActions, {
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
            { data: 'NomeEmpresa' },
            { data: 'Nome' },
            { data: 'UserName' },
            { data: 'Email' },
            { data: 'Ativo' },
            actionsColumn
        ]
    });

    $('#dataTable').dataTable(options);

    dart.dataTables.loadFormFilterEvents();

    // Delete
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

    // Reset Password
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

    $("#pesquisarEmpresa").click(function () {
        $("#modalEmpresaPrincipal").load(HOST_URL + "Empresa/SearchModal?CampoSelecionado=EmpresaPrincipal", function () {
            $("#modalEmpresaPrincipal").modal();
        });
    });

    $("#limparEmpresa").click(limparEmpresa);
})();

function setEmpresa(idEmpresa, razaoSocial, campo) {
    $("#" + campo).find("#razaoSocial").val(razaoSocial);
    $("#" + campo).find("#empresaId").val(idEmpresa);

    $("#modal" + campo).modal("hide");
    $("#modal" + campo).empty();
}

function limparEmpresa() {
    $("#EmpresaPrincipal").find("#razaoSocial").val("");
    $("#EmpresaPrincipal").find("#empresaId").val("");
}