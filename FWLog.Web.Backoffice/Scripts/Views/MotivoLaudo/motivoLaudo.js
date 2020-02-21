(function () {

    var actionsColumn = dart.dataTables.renderActionsColumn(function (data, type, full, meta) {
        return [
            {
                attrs: { 'data-id': full.IdMotivoLaudo, 'action': 'editarMotivoLaudo' },
                //href: view.editUrl + '?id=' + full.IdMotivoLaudo,
                icon: 'fa fa-edit',
                visible: view.editVisible
            },
        ];
    });

    $("#dataTable").on('click', "[action='editarMotivoLaudo']", editarMotivoLaudo);

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

function editarMotivoLaudo() {
    let id = $(this).data("id");

    $("#modalEditarMotivoLaudo").load(HOST_URL + CONTROLLER_PATH + "ExibirModalMotivoLaudo/" + id, function () {
        $("#modalEditarMotivoLaudo").modal();
        $('input').iCheck({ checkboxClass: 'icheckbox_flat-green' });
    });
}