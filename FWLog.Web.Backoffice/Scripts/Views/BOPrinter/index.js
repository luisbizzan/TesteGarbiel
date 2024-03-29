﻿(function () {

    var actionsColumn = dart.dataTables.renderActionsColumn(function (data, type, full, meta) {
        return [
            {
                action: 'details',
                href: view.detailsUrl + '?id=' + full.Id,
                visible: view.detailsVisible
            },
            {
                action: 'edit',
                href: view.editUrl + '?id=' + full.Id,
                visible: view.editVisible
            },
            {
                action: 'delete',
                attrs: { 'data-delete-url': view.deleteUrl + '?id=' + full.Id },
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
        columns: [
            { data: 'Name' },
            { data: 'PrinterType' },
            { data: 'Status', width: 100  },
            { data: 'Empresa' },
            actionsColumn
        ]
    };

    $('#dataTable').DataTable(options);

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